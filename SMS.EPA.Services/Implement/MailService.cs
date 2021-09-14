namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model;
    using SMS.EPA.Model.AutoService;
    using SMS.EPA.Model.Email;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;

    public class MailService : IMailService
    {
        private readonly IBaseMailService baseMailService;

        public MailService() : this(null)
        {
        }

        public MailService(IBaseMailService baseMailService)
        {
            this.baseMailService = baseMailService ?? new BaseMailService();
        }

        public void SendMailReminder(string emailTemplatePath, EmployeeSettingToSendMail employeeSetting, MailMode mailMode)
        {
            if (employeeSetting != null && !string.IsNullOrWhiteSpace(emailTemplatePath))
            {
                var bodyTemplate = baseMailService.ReadTextFile(emailTemplatePath);
                var emailReminder = new MailReminder(employeeSetting, bodyTemplate, mailMode);
                baseMailService.SendEmail(emailReminder.Email);
            }
        }

        public void SendMailToNotifyChangeSetting(string emailTemplatePath, EmployeeSettingToSendMail employeeSetting)
        {
            if (employeeSetting != null && !string.IsNullOrWhiteSpace(emailTemplatePath))
            {
                var email = new MailModel
                {
                    Subject = string.Format(Lang.NotifyEmailChangeSetting, employeeSetting.Period),
                    Body = baseMailService.ReadTextFile(emailTemplatePath)
                };
                email.Body = string.Format(
                    email.Body,
                    employeeSetting.Nickname,
                    employeeSetting.Period,
                    ConfigurationManagerService.GetAppSetting("url"));
                email.ReceiverEmail = employeeSetting.Email;

                baseMailService.SendEmail(email);
            }
        }

        public void SendMailToSelfEndEvaluation(
            string emailTemplatePath,
            EPAModel evaluationInfo,
            float? averagePerformanceScore,
            float disciplineScore)
        {
            if (evaluationInfo != null && !string.IsNullOrWhiteSpace(emailTemplatePath))
            {
                MailModel email = new MailModel
                {
                    Subject = string.Format(Lang.EndMailEvaluationToSelf, evaluationInfo.Period.Name)
                };

                float? finalScore = averagePerformanceScore + disciplineScore;
                float? adjustmentScore = averagePerformanceScore.HasValue
                        ? GetAdjustmentScore(evaluationInfo, averagePerformanceScore.Value)
                        : null;
                string evaluatorNames = GetEvaluator(evaluationInfo.Process.Evaluators);
                string reviewerNames = GetEvaluator(evaluationInfo.Process.Reviewers);
                string managementNames = GetEvaluator(evaluationInfo.Process.Managements);
                email.Body = baseMailService.ReadTextFile(emailTemplatePath);
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
                    ConfigurationManagerService.GetAppSetting("url"));
                email.ReceiverEmail = evaluationInfo.Employee.Email;

                baseMailService.SendEmail(email);
            }
        }

        public void SendMailEndToHumanResourcesByEmployee(
            string emailTemplatePath,
            EPAModel evaluationInfo,
            EPAProcessModel humanResource)
        {
            if (evaluationInfo != null && humanResource != null && !string.IsNullOrWhiteSpace(emailTemplatePath))
            {
                MailModel email = new MailModel
                {
                    Subject = string.Format(Lang.EndMailEvaluationToSelf, evaluationInfo.Period.Name),
                    Body = baseMailService.ReadTextFile(emailTemplatePath)
                };

                email.Body = string.Format(
                    email.Body,
                    humanResource.EvaluatorInfo.NickName,
                    evaluationInfo.Employee.NickName,
                    evaluationInfo.Employee.EmployeeId,
                    evaluationInfo.Period.Name,
                    ConfigurationManagerService.GetAppSetting("url"));
                email.ReceiverEmail = humanResource.EvaluatorInfo.Email;

                baseMailService.SendEmail(email);
            }
        }

        public void SendMailEndToHumanResourceByPeriod(string emailTemplatePath, EmployeeSettingToSendMail employeeSetting)
        {
            if (employeeSetting != null && !string.IsNullOrWhiteSpace(emailTemplatePath))
            {
                var email = new MailModel
                {
                    Subject = string.Format(Lang.EndMailEvaluationToSelf, employeeSetting.Period),
                    Body = baseMailService.ReadTextFile(emailTemplatePath)
                };
                email.Body = string.Format(email.Body, employeeSetting.Nickname, employeeSetting.Period);
                email.ReceiverEmail = employeeSetting.Email;

                baseMailService.SendEmail(email);
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
                adjustScore = averagePerformanceScore - evaluationInfo.Process.AverageScoreByReviewers ?? averagePerformanceScore;
            }
            else
            {
                if (evaluationInfo.Process.AverageScoreByReviewers.HasValue)
                {
                    adjustScore = averagePerformanceScore - evaluationInfo.Process.AverageScoreByEvaluators ?? averagePerformanceScore;
                }
            }

            return adjustScore;
        }
    }
}