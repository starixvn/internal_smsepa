namespace SMS.EPA.Model.Email
{
    using System;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Resources;

    public class MailReminder
    {
        private readonly int _processId;
        private readonly string _fromDate;
        private readonly string _toDate;
        private readonly string _periodName;

        public static string SenderName => ConfigurationManagerService.GetAppSetting("Email.SenderName");

        public static string SiteLink => ConfigurationManagerService.GetAppSetting("url");

        public MailReminder(PeriodFlowModel employeeSetting, string bodyTemplate, string periodName, int mailMode)
        {
            _processId = employeeSetting.ProcessId;
            _fromDate = employeeSetting.StartDate.HasValue
                ? ChangeStartDate(employeeSetting.StartDate.Value)
                : string.Empty;

            _toDate = employeeSetting.EndDate.HasValue
                ? ChangeEndDate(employeeSetting.EndDate.Value)
                : string.Empty;

            _periodName = periodName;

            Email = new MailModel
            {
                Subject = GetSubject(mailMode),
                Body = string.Format(
                    bodyTemplate,
                    employeeSetting.EvaluatorInfo.NickName,
                    SenderName,
                    periodName,
                    SiteLink,
                    _fromDate,
                    _toDate),
                ReceiverEmail = employeeSetting.EvaluatorInfo.Email
            };
        }

        public string PeriodName { get; private set; }

        public MailModel Email { get; }

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

        private string GetSubject(int mailMode)
        {
            string subject;
            switch (_processId)
            {
                case (int)ProcessType.HumanResource:
                    subject = mailMode == (int)MailMode.Start
                        ? Lang.StartDateEmailToHRSubject
                        : Lang.RemindDateEmailToHRSubject;
                    break;

                case (int)ProcessType.Self:
                    subject = mailMode == (int)MailMode.Start
                        ? Lang.StartDateEmailToSelfSubject
                        : Lang.RemindDateEmailToSelfSubject;
                    break;

                case (int)ProcessType.Evaluator:
                    subject = mailMode == (int)MailMode.Start
                        ? Lang.StartDateEmailToEvaluatorSubject
                        : Lang.RemindDateEmailToEvaluatorSubject;
                    break;

                case (int)ProcessType.Reviewer:
                    subject = mailMode == (int)MailMode.Start
                        ? Lang.StartDateEmailToReviewerSubject
                        : Lang.RemindDateEmailToReviewerSubject;
                    break;

                case (int)ProcessType.Management:
                    subject = mailMode == (int)MailMode.Start
                        ? Lang.StartDateEmailToManagementSubject
                        : Lang.RemindDateEmailToManagementSubject;
                    break;

                default:
                    subject = string.Empty;
                    break;
            }

            return string.Format(subject, _periodName, _fromDate, _toDate);
        }
    }
}