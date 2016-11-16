namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Interfaces;

    public class PeriodService : BaseService, IPeriodService
    {
        private readonly IPeriodRepository periodRepository;
        private readonly IManualMailService manualMailService;
        private readonly IConfigurationManagerService configurationManager;

        public PeriodService()
            : this(null, null, null)
        {
        }

        public PeriodService(
            IPeriodRepository periodRepository,
            IManualMailService manualMailService,
            IConfigurationManagerService configurationManager)
        {
            this.periodRepository = periodRepository ?? new PeriodRepository();
            this.manualMailService = manualMailService ?? new ManualMailService();
            this.configurationManager = configurationManager ?? new ConfigurationManagerService();
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

        public IEnumerable<PeriodModel> GetMonthlyPeriod(GetMonthlyPeriodCriteria criteria)
        {
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = GetData<PeriodModel>(criteria);
            }

            return periods;
        }

        public IEnumerable<PeriodModel> GetAllMonthlyPeriod()
        {
            var criteria = new GetAllMonthlyPeriodCriteria();
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria.IsValid())
            {
                periods = GetData<PeriodModel>(criteria);
            }

            return periods;
        }

        public CommandResult CreatePeriod(CreatePeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult AutoCreateMonthlyPeriod(AutoCreateMonthlyPeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeletePeriod(DeletePeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
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

        public CommandResult AddEmployeeToPeriod(AddEmployeeToPeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
                string sendMailWhenEditEmployeeSetting = configurationManager.GetAppSetting("SendMailWhenEditEmployeeSetting");

                if (commandResult.IsSuccessful && sendMailWhenEditEmployeeSetting.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    GetAndSendEmailToAllRelatedEmployee(MailMode.NotifyChange, criteria.PeriodId, criteria.EmployeeIds, null);
                }
            }

            return commandResult;
        }

        public CommandResult UpdateIncludeStatusEmployeePeriod(UpdateIncludeStatusEmployeePeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult GenerateEvaluation(AutoGenerateEvaluationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public void SendEmailNoticeAtBegin(int? periodId = null)
        {
            IEnumerable<AutoEmployeePeriodModel> startedProcess = GetPeriodHasStartedProcess(periodId);

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

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasStartedProcess(int? periodId)
        {
            var criteria = new GetPeriodHasStartedProcessCriteria
            {
                PeriodId = periodId
            };
            IEnumerable<AutoEmployeePeriodModel> startedPeriods = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return startedPeriods;
        }

        public IEnumerable<FlowModel> GetProcessSettingByPeriodId(int periodId)
        {
            var criteria = new GetProcessSettingPeriodCriteria
            {
                PeriodId = periodId
            };
            IEnumerable<FlowModel> processSettings = new List<FlowModel>();

            if (criteria.IsValid())
            {
                processSettings = GetData<FlowModel>(criteria);
            }

            return processSettings;
        }

        public IEnumerable<FlowModel> GetEvaluationGlobalSettingByTargetTypeId(int targetTypeId)
        {
            var criteria = new GetEvaluationGlobalSettingByTargetTypeIdCriteria
            {
                TargetTypeId = targetTypeId
            };
            IEnumerable<FlowModel> processSettings = new List<FlowModel>();

            if (criteria.IsValid())
            {
                processSettings = GetData<FlowModel>(criteria);
            }

            return processSettings;
        }

        public CommandResult UpdateProcessSettingPeriod(UpdateProcessSettingPeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public bool CheckSelfProcessStarted(CheckSelfProcessStartedCriteria criteria)
        {
            bool hasStarted = false;

            if (criteria != null && criteria.IsValid())
            {
                hasStarted = GetDataEntity<bool>(criteria);
            }

            return hasStarted;
        }

        private void GetAndSendEmailToAllRelatedEmployee(MailMode mailMode, int periodId, string employeeIds, int? processId)
        {
            PeriodModel period = GetPeriodById(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            switch (mailMode)
            {
                case MailMode.Start:
                    manualMailService.SendStartMail(period, employeeSettings);
                    break;

                case MailMode.NotifyChange:
                    manualMailService.SendNotifyMail(period, employeeSettings);
                    break;
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId
            };
            employeeSettings = GetEmployeeSettingByPeriodId(criteria);

            return employeeSettings;
        }
    }
}