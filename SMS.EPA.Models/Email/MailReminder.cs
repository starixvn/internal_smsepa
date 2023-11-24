namespace SMS.EPA.Model.Email
{
    using System;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.AutoService;
    using SMS.EPA.Resources;

    public class MailReminder
    {
        private readonly byte _processId;
        private readonly string _fromDate;
        private readonly string _toDate;

        public static string SenderName => ConfigurationManagerService.GetAppSetting("Email.SenderName");

        public static string SiteLink => ConfigurationManagerService.GetAppSetting("url");

        public MailReminder(EmployeeSettingToSendMail employeeSetting, string bodyTemplate, MailMode mailMode)
        {
            _processId = employeeSetting.ProcessId;
            _fromDate = employeeSetting.StartDate.HasValue
                ? ChangeStartDate(employeeSetting.StartDate.Value)
                : string.Empty;

            _toDate = employeeSetting.EndDate.HasValue
                ? ChangeEndDate(employeeSetting.EndDate.Value)
                : string.Empty;

            Email = new MailModel
            {
                Subject = GetSubject(employeeSetting.Period, mailMode),
                Body = string.Format(
                    bodyTemplate,
                    employeeSetting.Nickname,
                    SenderName,
                    employeeSetting.Period,
                    SiteLink,
                    _fromDate,
                    _toDate),
                ReceiverEmail = employeeSetting.Email
            };
        }

        public MailModel Email { get; }

        private static string ChangeStartDate(DateTime startDate)
        {
            var result = new DateTimeOffset(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0, TimeSpan.Zero);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        private static string ChangeEndDate(DateTime endDate)
        {
            var result = new DateTimeOffset(endDate.Year, endDate.Month, endDate.Day, 23, 59, 0, 0, TimeSpan.Zero);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        private string GetSubject(string period, MailMode mailMode)
        {
            string subject;
            switch (_processId)
            {
                case (byte)ProcessType.HumanResource:
                    subject = GetSubject(mailMode, Lang.StartDateEmailToHRSubject, Lang.RemindDateEmailToHRSubject);
                    break;

                case (byte)ProcessType.Self:
                    subject = GetSubject(mailMode, Lang.StartDateEmailToSelfSubject, Lang.RemindDateEmailToSelfSubject);
                    break;

                case (byte)ProcessType.Evaluator:
                    subject = GetSubject(mailMode, Lang.StartDateEmailToEvaluatorSubject, Lang.RemindDateEmailToEvaluatorSubject);
                    break;

                case (byte)ProcessType.Reviewer:
                    subject = GetSubject(mailMode, Lang.StartDateEmailToReviewerSubject, Lang.RemindDateEmailToReviewerSubject);
                    break;

                case (byte)ProcessType.Management:
                    subject = GetSubject(mailMode, Lang.StartDateEmailToManagementSubject, Lang.RemindDateEmailToManagementSubject);
                    break;

                default:
                    subject = string.Empty;
                    break;
            }

            return string.Format(subject, period, _fromDate, _toDate);
        }

        private static string GetSubject(MailMode mailMode, string start, string remind) => mailMode == MailMode.Start ? start : remind;
    }
}