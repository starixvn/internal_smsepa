namespace SMS.EPA.Model.Email
{
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Resources;
    using System;

    public class MailReminder
    {
        private readonly int processId;
        private readonly string fromDate;
        private readonly string toDate;
        private readonly string periodName;
        private readonly int mailMode;

        public static string SenderName
        {
            get
            {
                return ConfigurationManagerService.GetAppSetting("Email.SenderName");
            }
        }

        public static string SiteUrl
        {
            get
            {
                return ConfigurationManagerService.GetAppSetting("url");
            }
        }

        public string PeriodName { get; private set; }

        public MailModel Email { get; private set; }

        public MailReminder(PeriodFlowModel employeeSetting, string bodyTemplate, string periodName, int mailMode)
        {
            processId = employeeSetting.ProcessId;
            fromDate = employeeSetting.StartDate.HasValue
                    ? ChangeStartDate(employeeSetting.StartDate.Value)
                    : string.Empty;

            toDate = employeeSetting.EndDate.HasValue
                    ? ChangeEndDate(employeeSetting.EndDate.Value)
                    : string.Empty;

            this.periodName = periodName;
            this.mailMode = mailMode;

            Email = new MailModel
            {
                Subject = GetSubject(),
                Body = string.Format(bodyTemplate, employeeSetting.EvaluatorInfo.NickName, SenderName, periodName, SiteUrl, fromDate, toDate),
                ReceiverEmail = employeeSetting.EvaluatorInfo.Email
            };
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

        private string GetSubject()
        {
            switch (mailMode)
            {
                case (int)MailMode.Start:
                    return GetSubjectWhenStart();
     
                case (int)MailMode.Reminder:
                    return GetSubjectWhenReminder();

                default:
                    return string.Empty;
            }
        }
        
        private string GetSubjectWhenReminder()
        {
            string subject;
            switch (processId)
            {
                case (int)ProcessType.HumanResource:
                    subject = Lang.StartDateEmailToHRSubject;
                    break;

                case (int)ProcessType.Self:
                    subject = Lang.RemindDateEmailToSelfSubject;
                    break;

                case (int)ProcessType.Evaluator:
                    subject = Lang.RemindDateEmailToEvaluatorSubject;
                    break;

                case (int)ProcessType.Reviewer:
                    subject = Lang.RemindDateEmailToReviewerSubject;
                    break;

                case (int)ProcessType.Management:
                    subject = Lang.RemindDateEmailToManagementSubject;
                    break;

                default:
                    subject = string.Empty;
                    break;
            }

            return string.Format(subject, periodName, fromDate, toDate);
        }

        private string GetSubjectWhenStart()
        {
            string subject;
            switch (processId)
            {
                case (int)ProcessType.HumanResource:
                    subject = Lang.StartDateEmailToHRSubject;
                    break;

                case (int)ProcessType.Self:
                    subject = Lang.RemindDateEmailToSelfSubject;
                    break;

                case (int)ProcessType.Evaluator:
                    subject = Lang.RemindDateEmailToEvaluatorSubject;
                    break;

                case (int)ProcessType.Reviewer:
                    subject = Lang.RemindDateEmailToReviewerSubject;
                    break;

                case (int)ProcessType.Management:
                    subject = Lang.RemindDateEmailToManagementSubject;
                    break;

                default:
                    subject = string.Empty;
                    break;
            }

            return string.Format(subject, periodName, fromDate, toDate);
        }
    }
}
