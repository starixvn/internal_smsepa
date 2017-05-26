namespace SMS.EPA.Services.Implement
{
    using Core.Services;
    using Interfaces;
    using Model;
    using Model.Email;
    using Resources;
    using System.Collections.Generic;
    using System.Linq;

    public class MailService : IMailService
    {
        private readonly IBaseMailService _baseMailService;
        private readonly string url;

        public MailService()
            : this(null)
        {
        }

        public MailService(IBaseMailService baseMailService)
        {
            _baseMailService = baseMailService ?? new BaseMailService();
            url = ConfigurationManagerService.GetAppSetting("SendMailWhenEditEmployeeSetting");
        }

        public void SendMailReminder(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName, int mailMode)
        {
            if (employeeSetting != null && !string.IsNullOrWhiteSpace(templateEmailPath))
            {
                var bodyTemplate = _baseMailService.ReadTextFile(templateEmailPath);
                var emailReminder = new MailReminder(employeeSetting, bodyTemplate, periodName, mailMode);
                _baseMailService.SendEmail(emailReminder.Email);
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

                email.Body = string.Format(email.Body, employeeSetting.EvaluatorInfo.NickName, periodName, url);
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
                    managementNames,
                    url);
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
                    evaluationInfo.Period.Name,
                    url);
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