// ----------------------------------------------------------------------
// <copyright file="ManualMailService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/04/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Web;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Interfaces;

    public class ManualMailService : IManualMailService
    {
        private string _startDateMailToHumanResources = "~/Views/Emails/StartDateEmailToHR.cshtml";
        private string _startDateMailToSelf = "~/Views/Emails/StartDateEmailToSelf.cshtml";
        private string _startDateMailToEvaluator = "~/Views/Emails/StartDateEmailToEvaluator.cshtml";
        private string _startDateMailToSpecialEvaluator = "~/Views/Emails/StartDateEmailToSpecialEvaluator.cshtml";
        private string _startDateMailToReviewer = "~/Views/Emails/StartDateEmailToReviewer.cshtml";
        private string _startDateMailToSpecialReviewer = "~/Views/Emails/StartDateEmailToSpecialReviewer.cshtml";
        private string _startDateMailToManagement = "~/Views/Emails/StartDateEmailToManagement.cshtml";
        private string _startDateMailToSpecialManagement = "~/Views/Emails/StartDateEmailToSpecialManagement.cshtml";
        private string _mailNotifyChangeSetting = "~/Views/Emails/EmailNotifyChangeSetting.cshtml";

        private IMailService _mailService;

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
            string templateEmailPath = string.Empty;

            foreach (PeriodFlowModel employeeSetting in employeeSettings)
            {
                switch (employeeSetting.ProcessId)
                {
                    case (int)ProcessType.HumanResource:
                        templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToHumanResources);
                        _mailService.SendMailToHRStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Self:
                        templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToSelf);
                        _mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Evaluator:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToSpecialEvaluator);
                        }
                        else
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToEvaluator);
                        }

                        _mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Reviewer:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToSpecialReviewer);
                        }
                        else
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToReviewer);
                        }

                        _mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Management:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToSpecialManagement);
                        }
                        else
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToManagement);
                        }

                        _mailService.SendMailToManagementStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;
                }
            }
        }

        public void SendNotifyMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            string templateEmailPath = HttpContext.Current.Server.MapPath(_mailNotifyChangeSetting);

            foreach (PeriodFlowModel employeeSetting in employeeSettings)
            {
                _mailService.SendMailToNotifyChangeSetting(templateEmailPath, employeeSetting, period.Name);
            }
        }
    }
}
