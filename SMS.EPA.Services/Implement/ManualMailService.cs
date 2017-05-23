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

    public class ManualMailService : IManualMailService
    {
        private const string StartDateMailToHumanResources = "StartDateEmailToHR.cshtml";
        private const string StartDateMailToSelf = "StartDateEmailToSelf.cshtml";
        private const string StartDateMailToEvaluator = "StartDateEmailToEvaluator.cshtml";
        private const string StartDateMailToReviewer = "StartDateEmailToReviewer.cshtml";
        private const string StartDateMailToManagement = "StartDateEmailToManagement.cshtml";
        private const string MailNotifyChangeSetting = "EmailNotifyChangeSetting.cshtml";

        private readonly IMailService mailService;

        public ManualMailService()
            : this(null)
        {
        }

        public ManualMailService(IMailService mailService)
        {
            this.mailService = mailService ?? new MailService();
        }

        public void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    string templateEmailPath;

                    switch (employeeSetting.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            templateEmailPath = GetEmailPath(StartDateMailToHumanResources);
                            mailService.SendMailToHrStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Self:
                            templateEmailPath = GetEmailPath(StartDateMailToSelf);
                            mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = GetEmailPath(StartDateMailToEvaluator);
                            mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = GetEmailPath(StartDateMailToReviewer);
                            mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Management:
                            templateEmailPath = GetEmailPath(StartDateMailToManagement);
                            mailService.SendMailToManagementStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        default:
                            throw new ArgumentException("type not in case");
                    }
                }
            }
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

        private static string GetEmailPath(string fileName)
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            string emailPath = Path.Combine(assemblyPath, ConfigurationManagerService.GetAppSetting("RelativeEmailPath") + fileName);

            return emailPath;
        }
    }
}