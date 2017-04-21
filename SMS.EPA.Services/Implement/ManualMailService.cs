namespace SMS.EPA.Services.Implement
{
    using Core.Enumerations;
    using Interfaces;
    using Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ManualMailService : IManualMailService
    {
        private readonly IMailService _mailService;

        private string _startDateMailToHumanResources = "~/Views/Emails/StartDateEmailToHR.cshtml";
        private string _startDateMailToSelf = "~/Views/Emails/StartDateEmailToSelf.cshtml";
        private string _startDateMailToEvaluator = "~/Views/Emails/StartDateEmailToEvaluator.cshtml";
        private string _startDateMailToReviewer = "~/Views/Emails/StartDateEmailToReviewer.cshtml";
        private string _startDateMailToManagement = "~/Views/Emails/StartDateEmailToManagement.cshtml";
        private string _mailNotifyChangeSetting = "~/Views/Emails/EmailNotifyChangeSetting.cshtml";

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
                            templateEmailPath = GetFullPath(_startDateMailToHumanResources);
                            _mailService.SendMailToHrStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Self:
                            templateEmailPath = GetFullPath(_startDateMailToSelf);
                            _mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period.Name);
                            break;

                        case (int)ProcessType.Evaluator:
                            templateEmailPath = GetFullPath(_startDateMailToEvaluator);
                            _mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period.Name);

                            break;

                        case (int)ProcessType.Reviewer:
                            templateEmailPath = GetFullPath(_startDateMailToReviewer);
                            _mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period.Name);

                            break;

                        case (int)ProcessType.Management:
                            templateEmailPath = GetFullPath(_startDateMailToManagement);
                            _mailService.SendMailToManagementStartDate(templateEmailPath, employeeSetting, period.Name);

                            break;
                    }
                }
            }
        }

        public void SendNotifyMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            if (employeeSettings != null && employeeSettings.Any() && period != null)
            {
                string templateEmailPath = GetFullPath(_mailNotifyChangeSetting);

                foreach (PeriodFlowModel employeeSetting in employeeSettings)
                {
                    _mailService.SendMailToNotifyChangeSetting(templateEmailPath, employeeSetting, period.Name);
                }
            }
        }

        private string GetFullPath(string path)
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