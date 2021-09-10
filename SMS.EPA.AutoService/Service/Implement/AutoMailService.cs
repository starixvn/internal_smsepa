namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Services.Implement;
    using Services.Interfaces;
    using SMS.EPA.Model.AutoService;

    public class AutoMailService : IAutoMailService
    {
        private readonly IPeriodService periodService;
        private readonly IPeriodMailService periodMailService;

        public AutoMailService() : this(null, null)
        {
        }

        public AutoMailService(
            IPeriodService periodService,
            IPeriodMailService periodMailService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.periodMailService = periodMailService ?? new PeriodMailService();
        }

        public void CheckAndSendMail()
        {
            CheckSendNoticeAtBeginEmail();

            CheckSendReminderEmail();
        }

        private void CheckSendNoticeAtBeginEmail()
        {
            IEnumerable<EmployeeSettingToSendMail> employeeSettings = periodService.GetEmployeeSettingToSendMail(mailMode: (byte)MailMode.Start);
            periodMailService.SendReminderMail(employeeSettings, MailMode.Start);
        }

        private void CheckSendReminderEmail()
        {
            IEnumerable<EmployeeSettingToSendMail> employeeSettings = periodService.GetEmployeeSettingToSendMail(mailMode: (byte)MailMode.Reminder, isEvaluated: false);
            periodMailService.SendReminderMail(employeeSettings, MailMode.Reminder);
        }
    }
}