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
    using System.Configuration;
    using SMS.EPA.Core;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services.Implement;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Model.EmployeeEPA;
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
            string startDate = ChangeStartDate(employeeSetting.StartDate.Value);
            string endDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToHRSubject, periodName, startDate, endDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, email.Title, periodName, startDate, endDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToSelfStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            string startDate = ChangeStartDate(employeeSetting.StartDate.Value);
            string endDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToSelfSubject, periodName, startDate, endDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, email.Title, periodName, startDate, endDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToEvaluatorStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            string startDate = ChangeStartDate(employeeSetting.StartDate.Value);
            string endDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToEvaluatorSubject, periodName, startDate, endDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, email.Title, periodName, startDate, endDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToReviewerStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            string startDate = ChangeStartDate(employeeSetting.StartDate.Value);
            string endDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToReviewerSubject, periodName, startDate, endDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, email.Title, periodName, startDate, endDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToManagementStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            string startDate = ChangeStartDate(employeeSetting.StartDate.Value);
            string endDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.StartDateEmailToManagementSubject, periodName, startDate, endDate);
            email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, email.Title, periodName, startDate, endDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToSelfRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            string startDate = ChangeStartDate(employeeSetting.StartDate.Value);
            string endDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.RemindDateEmailToSelfSubject, periodName, startDate, endDate);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName, startDate, endDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToEvaluatorRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            string startDate = ChangeStartDate(employeeSetting.StartDate.Value);
            string endDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.RemindDateEmailToEvaluatorSubject, periodName, startDate, endDate);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, startDate, endDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToReviewerRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            string startDate = ChangeStartDate(employeeSetting.StartDate.Value);
            string endDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.RemindDateEmailToReviewerSubject, periodName, startDate, endDate);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, startDate, endDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToManagementRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            string startDate = ChangeStartDate(employeeSetting.StartDate.Value);
            string endDate = ChangeEndDate(employeeSetting.EndDate.Value);

            email.Subject = string.Format(Lang.RemindDateEmailToManagementSubject, periodName, startDate, endDate);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, startDate, endDate);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToNotifyChangeSetting(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            email.Subject = string.Format(Lang.NotifyEmailChangeSetting, periodName);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailToSelfEndEvaluation(
            string templateEmailPath,
            EPAModel evaluationInfo,
            float averagePerformanceScore,
            float disciplineScore)
        {
            var email = new MailModel();
            email.Subject = string.Format(Lang.EndMailEvaluationToSelf, evaluationInfo.Period.Name);

            float finalScore = averagePerformanceScore + disciplineScore;
            finalScore = finalScore >= 0 ? finalScore : 0;
            float adjustmentScore = GetAdjustmentScore(evaluationInfo, averagePerformanceScore);
            string evaluatorNames = GetEvaluator(evaluationInfo.Evaluators);
            string reviewerNames = GetEvaluator(evaluationInfo.Reviewers);
            string managementNames = GetEvaluator(evaluationInfo.Managements);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                evaluationInfo.Employee.NickName,
                evaluationInfo.Period.Name,
                disciplineScore,
                averagePerformanceScore,
                adjustmentScore,
                finalScore,
                evaluationInfo.Grade,
                evaluatorNames,
                reviewerNames,
                managementNames);
            email.ReceiverEmail = evaluationInfo.Employee.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailEndToHumanResourcesByEmployee(
            string templateEmailPath,
            EPAModel evaluationInfo,
            EPAProcessModel humanResource)
        {
            var email = new MailModel();

            email.Subject = string.Format(Lang.EndMailEvaluationToSelf, evaluationInfo.Period.Name);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, humanResource.NickName, evaluationInfo.Employee.NickName, evaluationInfo.Employee.EmployeeId, evaluationInfo.Period.Name);
            email.ReceiverEmail = humanResource.Email;

            _baseMailService.SendEmail(email);
        }

        public void SendMailEndToHumanResourceByPeriod(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            var email = new MailModel();

            email.Subject = string.Format(Lang.EndMailEvaluationToSelf, periodName);
            email.Body = _baseMailService.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName);
            email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

            _baseMailService.SendEmail(email);
        }

        public string ChangeStartDate(DateTime startDate)
        {
            var result = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0, startDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        public string ChangeEndDate(DateTime endDate)
        {
            var result = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 0, 0, endDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        private string GetEvaluator(IList<EPAProcessModel> evaluators)
        {
            string names = string.Empty;

            foreach (EPAProcessModel evaluator in evaluators)
            {
                names += !string.IsNullOrWhiteSpace(names) ? ", " + evaluator.NickName : evaluator.NickName;
            }

            return names;
        }

        private float GetAdjustmentScore(EPAModel evaluationInfo, float averagePerformanceScore)
        {
            float adjustScore = 0;

            if (evaluationInfo.AverageScoreByManagements > 0)
            {
                if (evaluationInfo.AverageScoreByReviewers > 0)
                {
                    adjustScore = averagePerformanceScore - evaluationInfo.AverageScoreByReviewers;
                }
                else
                {
                    adjustScore = averagePerformanceScore - evaluationInfo.AverageScoreByEvaluators;
                }
            }
            else if (evaluationInfo.AverageScoreByReviewers > 0)
            {
                adjustScore = averagePerformanceScore - evaluationInfo.AverageScoreByEvaluators;
            }

            return adjustScore;
        }
    }
}