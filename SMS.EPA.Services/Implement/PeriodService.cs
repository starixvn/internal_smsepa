namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;

    public class PeriodService : IPeriodService
    {
        private readonly IPeriodRepository periodRepository;
        private readonly IPeriodMailService manualMailService;
        private readonly IBaseService baseService;

        public PeriodService() : this(null, null, null)
        {
        }

        public PeriodService(
            IPeriodRepository periodRepository,
            IPeriodMailService manualMailService,
            IBaseService baseService)
        {
            this.periodRepository = periodRepository ?? new PeriodRepository();
            this.manualMailService = manualMailService ?? new PeriodMailService();
            this.baseService = baseService ?? new BaseService();
        }

        public IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria)
        {
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public PeriodModel GetPeriodById(int periodId)
        {
            var criteria = new GetPeriodByIdCriteria
            {
                PeriodId = periodId
            };
            var period = new PeriodModel();

            if (criteria.IsValid())
            {
                period = periodRepository.GetPeriodById(criteria);
            }

            return period;
        }

        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(GetPeriodEmployeeSettingCriteria criteria)
        {
            IEnumerable<PeriodEmployeeSettingModel> employeeSettings = new List<PeriodEmployeeSettingModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeSettings = periodRepository.GetPeriodEmployeeSettingById(criteria);
            }

            return employeeSettings;
        }

        public CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria)
        {
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            string sendMailWhenEditEmployeeSetting = ConfigurationManagerService.GetAppSetting("SendMailWhenEditEmployeeSetting");

            if (commandResult.IsSuccessful && sendMailWhenEditEmployeeSetting.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                GetAndSendEmailToAllRelatedEmployee(MailMode.NotifyChange, criteria.PeriodId, criteria.EmployeeIds, null);
            }

            return commandResult;
        }

        public void SendEmailNoticeAtBegin(int? periodId)
        {
            var criteria = new GetPeriodHasStartedProcessCriteria
            {
                PeriodId = periodId
            };
            IEnumerable<AutoEmployeePeriodModel> startedProcess = baseService.GetData<AutoEmployeePeriodModel>(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(MailMode.Start, periodProcess.PeriodId, string.Empty, periodProcess.ProcessId);
            }
        }

        public IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(GetEmployeeSettingByPeriodIdCriteria criteria)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeSettings = periodRepository.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }

        private void GetAndSendEmailToAllRelatedEmployee(MailMode mailMode, int periodId, string employeeIds, int? processId)
        {
            PeriodModel period = GetPeriodById(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            switch (mailMode)
            {
                case MailMode.Start:
                    manualMailService.SendReminderMail(period, employeeSettings, (int)MailMode.Start);
                    break;

                case MailMode.NotifyChange:
                    manualMailService.SendNotifyMail(period, employeeSettings);
                    break;

                case MailMode.None:
                case MailMode.Reminder:
                case MailMode.EndPeriod:
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

            var employeeSettings = GetEmployeeSettingByPeriodId(criteria);

            return employeeSettings;
        }
    }
}