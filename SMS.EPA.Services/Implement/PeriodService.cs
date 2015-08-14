// ----------------------------------------------------------------------
// <copyright file="PeriodService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class PeriodService : BaseService, IPeriodService
    {
        private readonly IPeriodRepository _periodRepository;

        public PeriodService()
            : this(null)
        {
        }

        public PeriodService(IPeriodRepository periodRepository)
        {
            _periodRepository = periodRepository ?? new PeriodRepository();
        }

        public IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria.IsValid())
            {
                periods = _periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public CommandResult CreatePeriod(CreatePeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public PeriodModel GetPeriodById(GetPeriodByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            PeriodModel period = new PeriodModel();

            if (criteria.IsValid())
            {
                period = _periodRepository.GetPeriodById(criteria);
            }

            return period;
        }

        public CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeletePeriod(DeletePeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(GetPeriodEmployeeSettingCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<PeriodEmployeeSettingModel> periodEmployeeSettings = new List<PeriodEmployeeSettingModel>();

            if (criteria.IsValid())
            {
                RawPeriodEmployeeSettingModel rawPeriodEmployeeSetting = _periodRepository.GetPeriodEmployeeSettingById(criteria);

                if (rawPeriodEmployeeSetting.Employees != null && rawPeriodEmployeeSetting.Employees.Count() > 0)
                {
                    periodEmployeeSettings = ConvertToPeriodEmployeeSetting(rawPeriodEmployeeSetting);
                }
            }

            return periodEmployeeSettings;
        }

        private static IEnumerable<PeriodEmployeeSettingModel> ConvertToPeriodEmployeeSetting(RawPeriodEmployeeSettingModel rawPeriodEmployeeSetting)
        {
            IEnumerable<PeriodEmployeeSettingModel> employeeSettings = rawPeriodEmployeeSetting.Employees;

            foreach (PeriodEmployeeSettingModel employeeSetting in employeeSettings)
            {
                GetFlowSetting(rawPeriodEmployeeSetting.FlowSettings, employeeSetting);
            }

            return employeeSettings;
        }

        private static void GetFlowSetting(
            IEnumerable<RawPeriodFlowSettingModel> rawFlowSettings,
            PeriodEmployeeSettingModel employeeSetting)
        {
            if (rawFlowSettings != null)
            {
                IEnumerable<RawPeriodFlowSettingModel> rawEmployeeFlowSettings =
                           rawFlowSettings.Where(x => x.PeriodId == employeeSetting.PeriodId && x.EmployeeId == employeeSetting.EmployeeId);

                foreach (RawPeriodFlowSettingModel rawFlowSetting in rawEmployeeFlowSettings)
                {
                    employeeSetting = GetEvaluators(rawFlowSetting, employeeSetting);
                }
            }
        }

        private static PeriodEmployeeSettingModel GetEvaluators(
            RawPeriodFlowSettingModel rawFlowSetting,
            PeriodEmployeeSettingModel employeeSetting)
        {
            switch (rawFlowSetting.ProcessId)
            {
                case (int)ProcessType.Evaluator:
                    PeriodFlowModel evaluator = CreateEvaluator(rawFlowSetting);
                    employeeSetting.Evaluators.Add(evaluator);
                    break;

                case (int)ProcessType.Reviewer:
                    PeriodFlowModel reviewer = CreateEvaluator(rawFlowSetting);
                    employeeSetting.Reviewers.Add(reviewer);
                    break;

                case (int)ProcessType.Management:
                    PeriodFlowModel management = CreateEvaluator(rawFlowSetting);
                    employeeSetting.Managements.Add(management);
                    break;
            }

            return employeeSetting;
        }

        private static PeriodFlowModel CreateEvaluator(RawPeriodFlowSettingModel rawFlowSetting)
        {
            var evaluator = new PeriodFlowModel
            {
                ProcessId = rawFlowSetting.ProcessId,
                Priority = rawFlowSetting.Priority,
                Duration = rawFlowSetting.Duration,
                RemindDay = rawFlowSetting.RemindDay,
                StartDate = rawFlowSetting.StartDate,
                EndDate = rawFlowSetting.EndDate,
                HasSendMail = rawFlowSetting.HasSendMail,
                EvaluatorInfo = rawFlowSetting.EvaluatorInfo
            };

            return evaluator;
        }
    }
}
