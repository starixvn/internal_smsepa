namespace SMS.EPA.Services.Implement
{
    using Core;
    using Core.Constants;
    using Core.Services.Implement;
    using Core.Services.Interfaces;
    using Interfaces;
    using Model;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

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

        public void SendMailToHrStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;
                string
                    endDate = employeeSetting.EndDate.HasValue
                        ? ChangeEndDate(employeeSetting.EndDate.Value)
                        : string.Empty;
                email.Subject = string.Format(Lang.StartDateEmailToHRSubject, periodName, startDate, endDate);
                email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    employeeSetting.EvaluatorInfo.NickName,
                    email.Title,
                    periodName,
                    startDate,
                    endDate);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToSelfStartDate(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;
                string endDate = employeeSetting.EndDate.HasValue
                    ? ChangeEndDate(employeeSetting.EndDate.Value)
                    : string.Empty;

                email.Subject = string.Format(Lang.StartDateEmailToSelfSubject, periodName, startDate, endDate);
                email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    employeeSetting.EvaluatorInfo.NickName,
                    email.Title,
                    periodName,
                    startDate,
                    endDate);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToEvaluatorStartDate(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;
                string
                    endDate = employeeSetting.EndDate.HasValue
                        ? ChangeEndDate(employeeSetting.EndDate.Value)
                        : string.Empty;
                email.Subject = string.Format(Lang.StartDateEmailToEvaluatorSubject, periodName, startDate, endDate);
                email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    employeeSetting.EvaluatorInfo.NickName,
                    email.Title,
                    periodName,
                    startDate,
                    endDate);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToReviewerStartDate(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;
                string
                    endDate = employeeSetting.EndDate.HasValue
                        ? ChangeEndDate(employeeSetting.EndDate.Value)
                        : string.Empty;
                email.Subject = string.Format(Lang.StartDateEmailToReviewerSubject, periodName, startDate, endDate);
                email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    employeeSetting.EvaluatorInfo.NickName,
                    email.Title,
                    periodName,
                    startDate,
                    endDate);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToManagementStartDate(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;
                string
                    endDate = employeeSetting.EndDate.HasValue
                        ? ChangeEndDate(employeeSetting.EndDate.Value)
                        : string.Empty;
                email.Subject = string.Format(Lang.StartDateEmailToManagementSubject, periodName, startDate, endDate);
                email.Title = ConfigurationManager.AppSettings["Email.SenderName"];
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    employeeSetting.EvaluatorInfo.NickName,
                    email.Title,
                    periodName,
                    startDate,
                    endDate);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToSelfRemindDate(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;
                string
                    endDate = employeeSetting.EndDate.HasValue
                        ? ChangeEndDate(employeeSetting.EndDate.Value)
                        : string.Empty;
                email.Subject = string.Format(Lang.RemindDateEmailToSelfSubject, periodName, startDate, endDate);
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    employeeSetting.EvaluatorInfo.NickName,
                    periodName,
                    startDate,
                    endDate);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToEvaluatorRemindDate(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;
                string
                    endDate = employeeSetting.EndDate.HasValue
                        ? ChangeEndDate(employeeSetting.EndDate.Value)
                        : string.Empty;
                email.Subject = string.Format(Lang.RemindDateEmailToEvaluatorSubject, periodName, startDate, endDate);
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, startDate, endDate);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToReviewerRemindDate(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;
                string
                    endDate = employeeSetting.EndDate.HasValue
                        ? ChangeEndDate(employeeSetting.EndDate.Value)
                        : string.Empty;
                email.Subject = string.Format(Lang.RemindDateEmailToReviewerSubject, periodName, startDate, endDate);
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, startDate, endDate);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToManagementRemindDate(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel();
                string startDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;
                string
                    endDate = employeeSetting.EndDate.HasValue
                        ? ChangeEndDate(employeeSetting.EndDate.Value)
                        : string.Empty;
                email.Subject = string.Format(Lang.RemindDateEmailToManagementSubject, periodName, startDate, endDate);
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, startDate, endDate);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToNotifyChangeSetting(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel
                {
                    Subject = string.Format(Lang.NotifyEmailChangeSetting, periodName),
                    Body = _baseMailService.ReadTextFile(templateEmailPath)
                };

                email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailToSelfEndEvaluation(
            string templateEmailPath,
            EPAModel evaluationInfo,
            float? averagePerformanceScore,
            float disciplineScore)
        {
            if (evaluationInfo != null)
            {
                MailModel email = new MailModel
                {
                    Subject = string.Format(Lang.EndMailEvaluationToSelf, evaluationInfo.Period.Name)
                };

                float? finalScore = averagePerformanceScore + disciplineScore;
                float?
                    adjustmentScore = averagePerformanceScore.HasValue
                        ? GetAdjustmentScore(evaluationInfo, averagePerformanceScore.Value)
                        : null;
                string evaluatorNames = GetEvaluator(evaluationInfo.Process.Evaluators);
                string
                    reviewerNames = GetEvaluator(evaluationInfo.Process.Reviewers);
                string
                    managementNames = GetEvaluator(evaluationInfo.Process.Managements);
                email.Body = _baseMailService.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    evaluationInfo.Employee.NickName,
                    evaluationInfo.Period.Name,
                    disciplineScore,
                    averagePerformanceScore?.ToString() ?? Lang.NotAvailable,
                    adjustmentScore?.ToString() ?? Lang.NotAvailable,
                    finalScore?.ToString() ?? Lang.NotAvailable,
                    evaluationInfo.Grade,
                    evaluatorNames,
                    reviewerNames,
                    managementNames);
                email.ReceiverEmail = evaluationInfo.Employee.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailEndToHumanResourcesByEmployee(
            string templateEmailPath,
            EPAModel evaluationInfo,
            EPAProcessModel humanResource)
        {
            if (evaluationInfo != null && humanResource != null)
            {
                MailModel email = new MailModel
                {
                    Subject = string.Format(Lang.EndMailEvaluationToSelf, evaluationInfo.Period.Name),
                    Body = _baseMailService.ReadTextFile(templateEmailPath)
                };

                email.Body = string.Format(
                    email.Body,
                    humanResource.EvaluatorInfo.NickName,
                    evaluationInfo.Employee.NickName,
                    evaluationInfo.Employee.EmployeeId,
                    evaluationInfo.Period.Name);
                email.ReceiverEmail = humanResource.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        public void SendMailEndToHumanResourceByPeriod(
            string templateEmailPath,
            PeriodFlowModel employeeSetting,
            string periodName)
        {
            if (employeeSetting != null)
            {
                MailModel email = new MailModel
                {
                    Subject = string.Format(Lang.EndMailEvaluationToSelf, periodName),
                    Body = _baseMailService.ReadTextFile(templateEmailPath)
                };

                email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName);
                email.ReceiverEmail = employeeSetting.EvaluatorInfo.Email;

                _baseMailService.SendEmail(email);
            }
        }

        private static string ChangeStartDate(DateTime startDate)
        {
            var result = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0, startDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        private static string ChangeEndDate(DateTime endDate)
        {
            var result = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 0, 0, endDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        private static string GetEvaluator(IList<EPAProcessModel> evaluators)
        {
            string names = string.Join(",", evaluators.Select(x => x.EvaluatorInfo.NickName));

            return names;
        }

        private static float? GetAdjustmentScore(EPAModel evaluationInfo, float averagePerformanceScore)
        {
            float? adjustScore = null;

            if (evaluationInfo.Process.Managements.Any() && evaluationInfo.Process.AverageScoreByManagements.HasValue)
            {
                adjustScore = averagePerformanceScore - evaluationInfo.Process.AverageScoreByReviewers ??
                              averagePerformanceScore;
            }
            else
            {
                if (evaluationInfo.Process.AverageScoreByReviewers.HasValue)
                {
                    adjustScore = averagePerformanceScore - evaluationInfo.Process.AverageScoreByEvaluators ??
                                  averagePerformanceScore;
                }
            }

            return adjustScore;
        }
    }
}