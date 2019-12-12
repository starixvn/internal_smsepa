namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using EPA.Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class AutoMailService : IAutoMailService
    {
        private readonly IPeriodService periodService;
        private readonly IPeriodMailService periodMailService;
        private readonly IBaseService baseService;

        public AutoMailService() : this(null, null, null)
        {
        }

        public AutoMailService(
            IPeriodService periodService,
            IPeriodMailService periodMailService,
            IBaseService baseService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.periodMailService = periodMailService ?? new PeriodMailService();
            this.baseService = baseService ?? new BaseService();
        }

        public void CheckAndSendMail()
        {
            CheckSendNoticeAtBeginEmail();

            CheckSendReminderEmail();
        }

        private void CheckSendNoticeAtBeginEmail()
        {
            IEnumerable<AutoEmployeePeriodModel> startedProcess = baseService.GetData<AutoEmployeePeriodModel>(new GetPeriodHasStartedProcessCriteria());

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(
                    periodProcess.PeriodId,
                    string.Empty,
                    periodProcess.ProcessId,
                    (int)MailMode.Start);
            }
        }

        private void CheckSendReminderEmail()
        {
            var criteria = new GetPeriodHasReminderProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> reminderProcess = GetPeriodHasReminderProcess(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in reminderProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(
                    periodProcess.PeriodId,
                    string.Empty,
                    periodProcess.ProcessId,
                    (int)MailMode.Reminder);
            }
        }

        private IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess(GetPeriodHasReminderProcessCriteria criteria)
        {
            IEnumerable<AutoEmployeePeriodModel> reminderPeriods = baseService.GetData<AutoEmployeePeriodModel>(criteria);

            return reminderPeriods;
        }

        private void GetAndSendEmailToAllRelatedEmployee(int periodId, string employeeIds, int? processId, int mailMode)
        {
            PeriodModel period = periodService.GetPeriodById(periodId);

            switch (mailMode)
            {
                case (int)MailMode.Start:
                    IEnumerable<PeriodFlowModel> startMailEmployeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);
                    periodMailService.SendReminderMail(period, startMailEmployeeSettings, (int)MailMode.Start);
                    break;

                case (int)MailMode.Reminder:
                    IEnumerable<PeriodFlowModel> reminderMailEmployeeSettings = GetEmployeeSettings(periodId, employeeIds, processId, false);
                    periodMailService.SendReminderMail(period, reminderMailEmployeeSettings, (int)MailMode.Reminder);
                    break;

                case (int)MailMode.EndPeriod:
                    IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);
                    periodMailService.SendEndEvaluationMailToHumanResourceByPeriod(period, employeeSettings);
                    break;

                default:
                    return;
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId)
        {
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId
            };

            IEnumerable<PeriodFlowModel> employeeSettings = periodService.GetEmployeeSettingByPeriodId(criteria);

            return employeeSettings;
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId, bool? isEvaluated)
        {
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId,
                IsEvaluated = isEvaluated
            };

            IEnumerable<PeriodFlowModel> employeeSettings = periodService.GetEmployeeSettingByPeriodId(criteria);

            return employeeSettings;
        }
    }
}