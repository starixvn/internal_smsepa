namespace SMS.EPA.AutoService.Service
{
    using System;
    using System.IO;
    using SMS.EPA.AutoService.Criteria.JobLevelStandard;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.AutoService;
    using SMS.EPA.Model.Email;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class JobLevelStandardService : IJobLevelStandardService
    {
        private const string StartMailTemplateToEmployee = "JobLevelStandard\\StartToEmployee.cshtml";
        private const string StartMailTemplateToEvaluator = "JobLevelStandard\\StartToEvaluator.cshtml";
        private const string StartMailTemplateToManagement = "JobLevelStandard\\StartToManagement.cshtml";

        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;

        public JobLevelStandardService() : this(null, null)
        {
        }

        public JobLevelStandardService(IBaseService baseService, IBaseMailService mailService)
        {
            this.baseService = baseService ?? new BaseService();
            this.mailService = mailService ?? new BaseMailService();
        }

        public void SendReminderEmail()
        {
            var sender = ConfigurationManagerService.GetAppSetting("Email.SenderName");
            var siteurl = ConfigurationManagerService.GetAppSetting("url");

            var criteria = new GetJobLevelStandardPeriodEmployeeSettingToSendMailCriteria();
            var employeeSettings = baseService.GetData<JobLevelStandardPeriodEmployeeSetting>(criteria);
            foreach (JobLevelStandardPeriodEmployeeSetting setting in employeeSettings)
            {
                var emailTemplatePath = GetEmailtemplatePath(setting.ProcessId);
                var bodyTemplate = mailService.ReadTextFile(emailTemplatePath);

                var model = new MailModel
                {
                    Subject = GetSubject(setting),
                    Body = string.Format(
                        bodyTemplate,
                        setting.Nickname,
                        sender,
                        setting.PeriodName,
                        siteurl,
                        setting.FromDate.Date.ToString(Constants.FormatDateTimeEmail),
                        setting.ToDate.AddDays(1).AddSeconds(-1).ToString(Constants.FormatDateTimeEmail)),
                    ReceiverEmail = setting.Email
                };

                mailService.SendEmail(model);
            }
        }

        private static string GetEmailtemplatePath(byte processId)
        {
            string emailtemplatePath;
            switch (processId)
            {
                case (byte)JobLevelAssessmentProcess.Employee:
                    emailtemplatePath = StartMailTemplateToEmployee;
                    break;

                case (byte)JobLevelAssessmentProcess.Evaluator:
                    emailtemplatePath = StartMailTemplateToEvaluator;
                    break;

                case (byte)JobLevelAssessmentProcess.Management:
                    emailtemplatePath = StartMailTemplateToManagement;
                    break;

                default:
                    emailtemplatePath = string.Empty;
                    break;
            }

            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(assemblyPath, ConfigurationManagerService.GetAppSetting("RelativeEmailPath") + emailtemplatePath);
        }

        private static string GetSubject(JobLevelStandardPeriodEmployeeSetting setting)
        {
            string subject;
            var fromDate = setting.FromDate.Date.ToString(Constants.FormatDateTimeEmail);
            var toDate = setting.ToDate.AddDays(1).AddSeconds(-1).ToString(Constants.FormatDateTimeEmail);
            switch (setting.ProcessId)
            {
                case (byte)JobLevelAssessmentProcess.Employee:
                    subject = string.Format("{0} Assessment Verification from {1} to {2}", setting.PeriodName, fromDate, toDate);
                    break;

                case (byte)JobLevelAssessmentProcess.Evaluator:
                    subject = string.Format("{0} Assessment from {1} to {2}", setting.PeriodName, fromDate, toDate);
                    break;

                case (byte)JobLevelAssessmentProcess.Management:
                    subject = string.Format("{0} Assessment Approval from {1} to {2}", setting.PeriodName, fromDate, toDate);
                    break;

                default:
                    subject = string.Empty;
                    break;
            }

            return subject;
        }
    }
}