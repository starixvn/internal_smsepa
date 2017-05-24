namespace SMS.EPA.Services.Implement
{
    using Core.Enumerations;
    using Core.Services;
    using Interfaces;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class PeriodMailService : IPeriodMailService
    {
        private readonly string _reminderMailToHumanResources = "RemindDateEmailToHR.cshtml";
        private readonly string _reminderMailToSelf = "RemindDateEmailToSelf.cshtml";
        private readonly string _reminderMailToEvaluator = "RemindDateEmailToEvaluator.cshtml";
        private readonly string _reminderMailToReviewer = "RemindDateEmailToReviewer.cshtml";
        private readonly string _reminderMailToManagement = "RemindDateEmailToManagement.cshtml";

        private const string MailNotifyChangeSetting = "EmailNotifyChangeSetting.cshtml";

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

        public void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings, int mailMode)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                foreach (PeriodFlowModel employeeSetting in employeeSettings.Where(x => x.HasSendMail))
                {
                    string templateEmailPath = GetEmailtemplatePath(employeeSetting.ProcessId);
                    mailService.SendMailReminder(templateEmailPath, employeeSetting, period.Name, mailMode);
                }
            }
        }

        private string GetEmailtemplatePath(int processId)
        {
            string emailtemplatePath;

            switch (processId)
            {
                case (int)ProcessType.HumanResource:
                    emailtemplatePath = _reminderMailToHumanResources;
                    break;
                case (int)ProcessType.Self:
                    emailtemplatePath = _reminderMailToSelf;
                    break;

                case (int)ProcessType.Evaluator:
                    emailtemplatePath = _reminderMailToEvaluator;
                    break;

                case (int)ProcessType.Reviewer:
                    emailtemplatePath = _reminderMailToReviewer;
                    break;

                case (int)ProcessType.Management:
                    emailtemplatePath = _reminderMailToManagement;
                    break;

                default:
                    emailtemplatePath = string.Empty;
                    break;
            }


            return GetEmailPath(emailtemplatePath);
        }

        public void SendNotifyMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string templateEmailPath = GetEmailPath(MailNotifyChangeSetting);

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    mailService.SendMailToNotifyChangeSetting(templateEmailPath, employeeSetting, period.Name);
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

        public void SendEndEvaluationMailToHumanResourceByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
                string templateEmailPath = Path.Combine(assemblyPath, _endMailToHumanReouscesByPeriod);

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    mailService.SendMailEndToHumanResourceByPeriod(templateEmailPath, employeeSetting, period.Name);
                }
            }
        }

        private static string GetEmailPath(string fileName)
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            string emailPath = Path.Combine(assemblyPath, ConfigurationManagerService.GetAppSetting("RelativeEmailPath") + fileName);

            return emailPath;
        }
    }
}