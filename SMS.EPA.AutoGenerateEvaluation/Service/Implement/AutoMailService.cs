namespace SMS.EPA.AutoService.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class AutoMailService : IAutoMailService
    {
        private string _startDateMailToHumanResources = "Views\\Emails\\StartDateEmailToHR.cshtml";
        private string _startDateMailToSelf = "Views\\Emails\\StartDateEmailToSelf.cshtml";
        private string _startDateMailToEvaluator = "Views\\Emails\\StartDateEmailToEvaluator.cshtml";
        private string _startDateMailToReviewer = "Views\\Emails\\StartDateEmailToReviewer.cshtml";
        private string _startDateMailToManagement = "Views\\Emails\\StartDateEmailToManagement.cshtml";

        private string _reminderMailToSelf = "Views\\Emails\\RemindDateEmailToSelf.cshtml";
        private string _reminderMailToEvaluator = "Views\\Emails\\RemindDateEmailToEvaluator.cshtml";
        private string _reminderMailToReviewer = "Views\\Emails\\RemindDateEmailToReviewer.cshtml";
        private string _reminderMailToManagement = "Views\\Emails\\RemindDateEmailToManagement.cshtml";

        private string _endMailToSelf = "Views\\Emails\\EndEvaluationEmailToSelf.cshtml";
        private string _endMailToHumanResourcesByEmployee = "Views\\Emails\\EndEvaluationEmailToHRByEmployee.cshtml";
        private string _endMailToHumanReouscesByPeriod = "Views\\Emails\\EndEvaluationEmailToHRByPeriod.cshtml";

        private IMailService mailService;

        public AutoMailService()
            : this(null)
        {
        }

        public AutoMailService(IMailService mailService)
        {
            this.mailService = mailService ?? new MailService();
        }

        public void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string templateEmailPath = string.Empty,
                    assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

                foreach (PeriodFlowModel employeeSetting in employeeSettings.Where(x => x.HasSendMail))
                {
                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToHumanResources);
                            mailService.SendMailToHRStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Self:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToSelf);
                            mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToEvaluator);
                            mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToReviewer);
                            mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Management:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToManagement);
                            mailService.SendMailToManagementStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;
                    }
                }
            }
        }

        public void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string templateEmailPath = string.Empty,
                    assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.Self:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToSelf);
                            mailService.SendMailToSelfRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToEvaluator);
                            mailService.SendMailToEvaluatorRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToReviewer);
                            mailService.SendMailToReviewerRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Management:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToManagement);
                            mailService.SendMailToManagementRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;
                    }
                }
            }
        }

        public void SendEndEvaluationMailToSelf(
            EPAModel evaluationInfo,
            float? averagePerformanceScore,
            float disciplineScore)
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory,
                templateEmailPath = Path.Combine(assemblyPath, _endMailToSelf);

            mailService.SendMailToSelfEndEvaluation(templateEmailPath, evaluationInfo, averagePerformanceScore, disciplineScore);
        }

        public void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo)
        {
            if (evaluationInfo != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory,
                    templateEmailPath = Path.Combine(assemblyPath, _endMailToHumanResourcesByEmployee);

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
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory,
                    templateEmailPath = Path.Combine(assemblyPath, _endMailToHumanReouscesByPeriod);

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    mailService.SendMailEndToHumanResourceByPeriod(templateEmailPath, employeeSetting, period.Name);
                }
            }
        }
    }
}