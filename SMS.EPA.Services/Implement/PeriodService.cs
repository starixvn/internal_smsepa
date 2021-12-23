namespace SMS.EPA.Services
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.AutoService;
    using SMS.EPA.Repositories;

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
            string isEnableSetting = ConfigurationManagerService.GetAppSetting("SendMailWhenEditEmployeeSetting");

            if (commandResult.IsSuccessful && isEnableSetting.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                IEnumerable<EmployeeSettingToSendMail> employeeSettings =
                    GetEmployeeSettingToSendMail(criteria.PeriodId, employeeIds: criteria.EmployeeIds);
                manualMailService.SendNotifyMail(employeeSettings);
            }

            return commandResult;
        }

        public void SendEmailNoticeAtBegin(int? periodId = null)
        {
            IEnumerable<EmployeeSettingToSendMail> employeeSettings = GetEmployeeSettingToSendMail(periodId, mailMode: (byte)MailMode.Start);
            manualMailService.SendReminderMail(employeeSettings, MailMode.Start);
        }

        public IEnumerable<EmployeeSettingToSendMail> GetEmployeeSettingToSendMail(
            int? periodId = null,
            byte? processId = null,
            byte? mailMode = null,
            string employeeIds = null,
            bool? isEvaluated = null)
        {
            var criteria = new GetEmployeeSettingToSendMailCriteria
            {
                PeriodId = periodId,
                ProcessId = processId,
                MailMode = mailMode,
                EmployeeIds = employeeIds,
                IsEvaluated = isEvaluated
            };
            return baseService.GetData<EmployeeSettingToSendMail>(criteria);
        }
    }
}