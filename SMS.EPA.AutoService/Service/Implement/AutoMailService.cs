namespace SMS.EPA.AutoService.Service
{
    using Core.Enumerations;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class AutoMailService : IAutoMailService
    {
        private readonly IMailService mailService;

        private readonly string _reminderMailToHumanResources = "RemindDateEmailToHR.cshtml";
        private readonly string _reminderMailToSelf = "RemindDateEmailToSelf.cshtml";
        private readonly string _reminderMailToEvaluator = "RemindDateEmailToEvaluator.cshtml";
        private readonly string _reminderMailToReviewer = "RemindDateEmailToReviewer.cshtml";
        private readonly string _reminderMailToManagement = "RemindDateEmailToManagement.cshtml";

        private readonly string _endMailToSelf = "EndEvaluationEmailToSelf.cshtml";
        private readonly string _endMailToHumanResourcesByEmployee = "EndEvaluationEmailToHRByEmployee.cshtml";
        private readonly string _endMailToHumanReouscesByPeriod = "EndEvaluationEmailToHRByPeriod.cshtml";

        public AutoMailService()
            : this(null)
        {
        }

        public AutoMailService(IMailService mailService)
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
            string emailtemplatePath ;

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
            string templateEmailPath = Path.Combine(assemblyPath, AppSetting.RelativeEmailPath + fileName);
            return templateEmailPath;
        }
    }
}