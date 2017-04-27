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
        private readonly IMailService _mailService;

        private readonly string _startDateMailToHumanResources = "Views\\Emails\\StartDateEmailToHR.cshtml";
        private readonly string _startDateMailToSelf = "Views\\Emails\\StartDateEmailToSelf.cshtml";
        private readonly string _startDateMailToEvaluator = "Views\\Emails\\StartDateEmailToEvaluator.cshtml";
        private readonly string _startDateMailToReviewer = "Views\\Emails\\StartDateEmailToReviewer.cshtml";
        private readonly string _startDateMailToManagement = "Views\\Emails\\StartDateEmailToManagement.cshtml";

        private readonly string _reminderMailToSelf = "Views\\Emails\\RemindDateEmailToSelf.cshtml";
        private readonly string _reminderMailToEvaluator = "Views\\Emails\\RemindDateEmailToEvaluator.cshtml";
        private readonly string _reminderMailToReviewer = "Views\\Emails\\RemindDateEmailToReviewer.cshtml";
        private readonly string _reminderMailToManagement = "Views\\Emails\\RemindDateEmailToManagement.cshtml";

        private readonly string _endMailToSelf = "Views\\Emails\\EndEvaluationEmailToSelf.cshtml";
        private readonly string _endMailToHumanResourcesByEmployee = "Views\\Emails\\EndEvaluationEmailToHRByEmployee.cshtml";
        private readonly string _endMailToHumanReouscesByPeriod = "Views\\Emails\\EndEvaluationEmailToHRByPeriod.cshtml";

        public AutoMailService()
            : this(null)
        {
        }

        public AutoMailService(IMailService mailService)
        {
            _mailService = mailService ?? new MailService();
        }

        public void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

                foreach (PeriodFlowModel employeeSetting in employeeSettings.Where(x => x.HasSendMail))
                {
                    string templateEmailPath;

                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToHumanResources);
                            _mailService.SendMailToHrStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Self:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToSelf);
                            _mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToEvaluator);
                            _mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToReviewer);
                            _mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Management:
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToManagement);
                            _mailService.SendMailToManagementStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;
                        default:
                            continue;
                    }
                }
            }
        }

        public void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    string templateEmailPath;

                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.Self:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToSelf);
                            _mailService.SendMailToSelfRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToEvaluator);
                            _mailService.SendMailToEvaluatorRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToReviewer);
                            _mailService.SendMailToReviewerRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Management:
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToManagement);
                            _mailService.SendMailToManagementRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;
                        default:
                            continue;
                    }
                }
            }
        }

        public void SendEndEvaluationMailToSelf(
            EPAModel evaluationInfo,
            float? averagePerformanceScore,
            float disciplineScore)
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            string templateEmailPath = Path.Combine(assemblyPath, _endMailToSelf);

            _mailService.SendMailToSelfEndEvaluation(templateEmailPath, evaluationInfo, averagePerformanceScore, disciplineScore);
        }

        public void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo)
        {
            if (evaluationInfo != null)
            {
                string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
                string templateEmailPath = Path.Combine(assemblyPath, _endMailToHumanResourcesByEmployee);

                foreach (EPAProcessModel humanResource in evaluationInfo.Process.HumanResources)
                {
                    _mailService.SendMailEndToHumanResourcesByEmployee(templateEmailPath, evaluationInfo, humanResource);
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
                    _mailService.SendMailEndToHumanResourceByPeriod(templateEmailPath, employeeSetting, period.Name);
                }
            }
        }
    }
}