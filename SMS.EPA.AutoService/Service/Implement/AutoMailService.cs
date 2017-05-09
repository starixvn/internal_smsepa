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

        private readonly string _startDateMailToHumanResources = "StartDateEmailToHR.cshtml";
        private readonly string _startDateMailToSelf = "StartDateEmailToSelf.cshtml";
        private readonly string _startDateMailToEvaluator = "StartDateEmailToEvaluator.cshtml";
        private readonly string _startDateMailToReviewer = "StartDateEmailToReviewer.cshtml";
        private readonly string _startDateMailToManagement = "StartDateEmailToManagement.cshtml";

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

        public void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                foreach (PeriodFlowModel employeeSetting in employeeSettings.Where(x => x.HasSendMail))
                {
                    string templateEmailPath;

                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            templateEmailPath = GetEmailPath(_startDateMailToHumanResources);
                            mailService.SendMailToHrStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Self:
                            templateEmailPath = GetEmailPath(_startDateMailToSelf);
                            mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = GetEmailPath(_startDateMailToEvaluator);
                            mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = GetEmailPath(_startDateMailToReviewer);
                            mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Management:
                            templateEmailPath = GetEmailPath(_startDateMailToManagement);
                            mailService.SendMailToManagementStartDate(templateEmailPath, employeeSetting, period.Name);
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
                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    string templateEmailPath;

                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.Self:
                            templateEmailPath = GetEmailPath(_reminderMailToSelf);
                            mailService.SendMailToSelfRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = GetEmailPath(_reminderMailToEvaluator);
                            mailService.SendMailToEvaluatorRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = GetEmailPath(_reminderMailToReviewer);
                            mailService.SendMailToReviewerRemindDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Management:
                            templateEmailPath = GetEmailPath(_reminderMailToManagement);
                            mailService.SendMailToManagementRemindDate(templateEmailPath, employeeSetting, period.Name);
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