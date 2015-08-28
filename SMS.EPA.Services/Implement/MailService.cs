// ----------------------------------------------------------------------
// <copyright file="MailService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/28/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SMS.EPA.Core.Services.Implement;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;

    public class MailService : IMailService
    {
        private readonly IBaseMailService _baseMailService;

        private string _startDateMailToHumanResources = "~/Views/Emails/StartDateEmailToHR.cshtml";
        private string _startDateMailToSelf = "~/Views/Emails/StartDateEmailToSelf.cshtml";
        private string _startDateMailToEvaluator = "~/Views/Emails/StartDateEmailToEvaluator.cshtml";
        private string _startDateMailToSpecialEvaluator = "~/Views/Emails/StartDateEmailToSpecialEvaluator.cshtml";
        private string _startDateMailToReviewer = "~/Views/Emails/StartDateEmailToReviewer.cshtml";
        private string _startDateMailToSpecialReviewer = "~/Views/Emails/StartDateEmailToSpecialReviewer.cshtml";
        private string _startDateMailToManagement = "~/Views/Emails/StartDateEmailToManagement.cshtml";
        private string _startDateMailToSpecialManagement = "~/Views/Emails/StartDateEmailToSpecialManagement.cshtml";

        public MailService()
            : this(null)
        {
        }

        public MailService(IBaseMailService baseMailService)
        {
            _baseMailService = baseMailService ?? new BaseMailService();
        }

        public void SendMailToHRStartDate(PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();
            email.Subject = string.Format(Lang.StartDateEmailToHRSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Body = _baseMailService.ReadTextFile(_startDateMailToHumanResources);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
        }

        public void SendMailToSelfStartDate(PeriodFlowModel employeeSetting, string periodName)
        {
            throw new NotImplementedException();
        }

        public void SendMailToEvaluatorStartDate(PeriodFlowModel employeeSetting, string periodName)
        {
            throw new NotImplementedException();
        }

        public void SendMailToReviewerStartDate(PeriodFlowModel employeeSetting, string periodName)
        {
            throw new NotImplementedException();
        }

        public void SendMailToManagementStartDate(PeriodFlowModel employeeSetting, string periodName)
        {
            throw new NotImplementedException();
        }

        public void SendMailToSelfRemindDate(PeriodFlowModel employeeSetting, string periodName)
        {
            throw new NotImplementedException();
        }

        public void SendMailToEvaluatorRemindDate(PeriodFlowModel employeeSetting, string periodName)
        {
            throw new NotImplementedException();
        }

        public void SendMailToReviewerRemindDate(PeriodFlowModel employeeSetting, string periodName)
        {
            throw new NotImplementedException();
        }

        public void SendMailToManagementRemindDate(PeriodFlowModel employeeSetting, string periodName)
        {
            throw new NotImplementedException();
        }
    }
}
