namespace SMS.EPA.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core.Enumerations;
    using Core.Services;
    using Model;
    using SMS.EPA.Model.AutoService;

    public class PeriodMailService : IPeriodMailService
    {
        private const string MailNotifyChangeSetting = "EmailNotifyChangeSetting.cshtml";

        private readonly string _reminderMailToHumanResources = "RemindDateEmailToHR.cshtml";
        private readonly string _reminderMailToSelf = "RemindDateEmailToSelf.cshtml";
        private readonly string _reminderMailToEvaluator = "RemindDateEmailToEvaluator.cshtml";
        private readonly string _reminderMailToReviewer = "RemindDateEmailToReviewer.cshtml";
        private readonly string _reminderMailToManagement = "RemindDateEmailToManagement.cshtml";

        private readonly string _endMailToSelf = "EndEvaluationEmailToSelf.cshtml";
        private readonly string _endMailToHumanResourcesByEmployee = "EndEvaluationEmailToHRByEmployee.cshtml";
        private readonly string _endMailToHumanReouscesByPeriod = "EndEvaluationEmailToHRByPeriod.cshtml";

        private readonly IMailService mailService;

        public PeriodMailService()
            : this(null)
        {
        }

        public PeriodMailService(IMailService mailService)
        {
            this.mailService = mailService ?? new MailService();
        }

        public void SendReminderMail(IEnumerable<EmployeeSettingToSendMail> employeeSettings, MailMode mailMode)
        {
            if (employeeSettings != null && employeeSettings.Any())
            {
                IEnumerable<EmployeeSettingToSendMail> settings = employeeSettings.Where(x => x.HasSendMail).ToList();
                foreach (EmployeeSettingToSendMail setting in settings)
                {
                    string emailTemplatePath = GetEmailtemplatePath(setting.ProcessId);
                    mailService.SendMailReminder(emailTemplatePath, setting, mailMode);
                }
            }
        }

        public void SendNotifyMail(IEnumerable<EmployeeSettingToSendMail> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any())
            {
                string emailTemplatePath = GetEmailPath(MailNotifyChangeSetting);
                foreach (EmployeeSettingToSendMail employeeSetting in employeeSettings)
                {
                    mailService.SendMailToNotifyChangeSetting(emailTemplatePath, employeeSetting);
                }
            }
        }

        public void SendEndEvaluationMailToSelf(
            EPAModel evaluationInfo,
            float? averagePerformanceScore,
            float disciplineScore)
        {
            string templateEmailPath = GetEmailPath(_endMailToSelf);

            mailService.SendMailToSelfEndEvaluation(templateEmailPath, evaluationInfo, averagePerformanceScore, disciplineScore);
        }

        public void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo)
        {
            if (evaluationInfo != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
                string templateEmailPath = Path.Combine(assemblyPath, _endMailToHumanResourcesByEmployee);

                foreach (EPAProcessModel humanResource in evaluationInfo.Process.HumanResources)
                {
                    mailService.SendMailEndToHumanResourcesByEmployee(templateEmailPath, evaluationInfo, humanResource);
                }
            }
        }

        public void SendEndEvaluationMailToHumanResourceByPeriod(IEnumerable<EmployeeSettingToSendMail> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any())
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
                string emailTemplatePath = Path.Combine(assemblyPath, _endMailToHumanReouscesByPeriod);
                foreach (EmployeeSettingToSendMail employeeSetting in employeeSettings)
                {
                    mailService.SendMailEndToHumanResourceByPeriod(emailTemplatePath, employeeSetting);
                }
            }
        }

        private static string GetEmailPath(string fileName)
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(assemblyPath, ConfigurationManagerService.GetAppSetting("RelativeEmailPath") + fileName);
        }

        private string GetEmailtemplatePath(byte processId)
        {
            string emailtemplatePath;
            switch (processId)
            {
                case (byte)ProcessType.HumanResource:
                    emailtemplatePath = _reminderMailToHumanResources;
                    break;

                case (byte)ProcessType.Self:
                    emailtemplatePath = _reminderMailToSelf;
                    break;

                case (byte)ProcessType.Evaluator:
                    emailtemplatePath = _reminderMailToEvaluator;
                    break;

                case (byte)ProcessType.Reviewer:
                    emailtemplatePath = _reminderMailToReviewer;
                    break;

                case (byte)ProcessType.Management:
                    emailtemplatePath = _reminderMailToManagement;
                    break;

                default:
                    emailtemplatePath = string.Empty;
                    break;
            }

            return GetEmailPath(emailtemplatePath);
        }
    }
}