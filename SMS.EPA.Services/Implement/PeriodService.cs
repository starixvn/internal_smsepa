namespace SMS.EPA.Services.Implement
{
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;
    using System;
    using System.Collections.Generic;

    public class PeriodService : BaseService, IPeriodService
    {
        private readonly IPeriodRepository periodRepository;
        private readonly IPeriodMailService manualMailService;

        public PeriodService() : this(null, null)
        {
        }

        public PeriodService(
            IPeriodRepository periodRepository,
            IPeriodMailService manualMailService)
        {
            this.periodRepository = periodRepository ?? new PeriodRepository();
            this.manualMailService = manualMailService ?? new PeriodMailService();
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
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult AutoCreateMonthlyPeriod(AutoCreateMonthlyPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeletePeriod(DeletePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

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
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
                string sendMailWhenEditEmployeeSetting = ConfigurationManagerService.GetAppSetting("SendMailWhenEditEmployeeSetting");

                if (commandResult.IsSuccessful && sendMailWhenEditEmployeeSetting.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    GetAndSendEmailToAllRelatedEmployee(MailMode.NotifyChange, criteria.PeriodId, criteria.EmployeeIds, null);
                }
            }

            return commandResult;
        }

        public CommandResult UpdateIncludeStatusEmployeePeriod(UpdateIncludeStatusEmployeePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

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

        public void SendEmailNoticeAtBegin(int? periodId)
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

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasStartedProcess(int? periodId = null)
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
            var commandResult = new CommandResult();

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
                    throw new ArgumentOutOfRangeException(nameof(mailMode), mailMode, null);
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