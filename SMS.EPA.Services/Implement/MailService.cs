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
using System.Configuration;
using SMS.EPA.Core;
using SMS.EPA.Core.Services.Implement;
using SMS.EPA.Core.Services.Interfaces;
using SMS.EPA.Model.Period;
using SMS.EPA.Resources;
using SMS.EPA.Services.Interfaces;

    public class MailService : IMailService
    {
        private readonly IBaseMailService _baseMailService;

        public MailService()
            : this(null)
        {
        }

        public MailService(IBaseMailService baseMailService)
        {
            _baseMailService = baseMailService ?? new BaseMailService();
        }

        public void SendMailToHRStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            employeeSetting.StartDate = ChangeStartDate(employeeSetting.StartDate.Value);
            employeeSetting.EndDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToHRSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, email.Title, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToSelfStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            employeeSetting.StartDate = ChangeStartDate(employeeSetting.StartDate.Value);
            employeeSetting.EndDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToSelfSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, "Josh", email.Title, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToEvaluatorStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            employeeSetting.StartDate = ChangeStartDate(employeeSetting.StartDate.Value);
            employeeSetting.EndDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToEvaluatorSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, email.Title, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToReviewerStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            employeeSetting.StartDate = ChangeStartDate(employeeSetting.StartDate.Value);
            employeeSetting.EndDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToReviewerSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, email.Title, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToManagementStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            employeeSetting.StartDate = ChangeStartDate(employeeSetting.StartDate.Value);
            employeeSetting.EndDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToManagementSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, email.Title, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToSelfRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            employeeSetting.StartDate = ChangeStartDate(employeeSetting.StartDate.Value);
            employeeSetting.EndDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.RemindDateEmailToSelfSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToEvaluatorRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            employeeSetting.StartDate = ChangeStartDate(employeeSetting.StartDate.Value);
            employeeSetting.EndDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.RemindDateEmailToEvaluatorSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToReviewerRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            employeeSetting.StartDate = ChangeStartDate(employeeSetting.StartDate.Value);
            employeeSetting.EndDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.RemindDateEmailToReviewerSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToManagementRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            employeeSetting.StartDate = ChangeStartDate(employeeSetting.StartDate.Value);
            employeeSetting.EndDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.RemindDateEmailToManagementSubject, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName, employeeSetting.StartDate, employeeSetting.EndDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public DateTime ChangeStartDate(DateTime startDate)
        {
            return new DateTime(startDate.Year,startDate.Month, startDate.Day, 0, 0, 0, 0, startDate.Kind);
        }

        public DateTime ChangeEndDate(DateTime endDate)
        {
            return new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 0, 0, endDate.Kind);
        }
    }
}
