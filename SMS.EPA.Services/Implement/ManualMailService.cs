namespace SMS.EPA.Services.Implement
{
    using System;
    using Core.Enumerations;
    using Interfaces;
    using Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ManualMailService : IManualMailService
    {
        private readonly IMailService _mailService;

        private const string StartDateMailToHumanResources = "~/Views/Emails/StartDateEmailToHR.cshtml";
        private const string StartDateMailToSelf = "~/Views/Emails/StartDateEmailToSelf.cshtml";
        private const string StartDateMailToEvaluator = "~/Views/Emails/StartDateEmailToEvaluator.cshtml";
        private const string StartDateMailToReviewer = "~/Views/Emails/StartDateEmailToReviewer.cshtml";
        private const string StartDateMailToManagement = "~/Views/Emails/StartDateEmailToManagement.cshtml";
        private const string MailNotifyChangeSetting = "~/Views/Emails/EmailNotifyChangeSetting.cshtml";

        public ManualMailService()
            : this(null)
        {
        }

        public ManualMailService(IMailService mailService)
        {
            _mailService = mailService ?? new MailService();
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
                            templateEmailPath = GetFullPath(StartDateMailToHumanResources);
                            _mailService.SendMailToHrStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Self:
                            templateEmailPath = GetFullPath(StartDateMailToSelf);
                            _mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = GetFullPath(StartDateMailToEvaluator);
                            _mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = GetFullPath(StartDateMailToReviewer);
                            _mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Management:
                            templateEmailPath = GetFullPath(StartDateMailToManagement);
                            _mailService.SendMailToManagementStartDate(templateEmailPath, employeeSetting, period.Name);
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
                string templateEmailPath = GetFullPath(MailNotifyChangeSetting);

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    _mailService.SendMailToNotifyChangeSetting(templateEmailPath, employeeSetting, period.Name);
                }
            }
        }

        private static string GetFullPath(string path)
        {
            string fullPath = string.Empty;
            if (!string.IsNullOrWhiteSpace(path) && HttpContext.Current != null)
            {
                fullPath = HttpContext.Current.Server.MapPath(path);
            }

            return fullPath;
        }
    }
}