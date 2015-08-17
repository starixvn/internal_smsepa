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
    using System;
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

        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(GetPeriodEmployeeSettingCriteria criteria, string startDate)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<PeriodEmployeeSettingModel> periodEmployeeSettings = new List<PeriodEmployeeSettingModel>();

            if (criteria.IsValid())
            {
                RawPeriodEmployeeSettingModel rawPeriodEmployeeSetting = _periodRepository.GetPeriodEmployeeSettingById(criteria);

                if (rawPeriodEmployeeSetting.Employees != null && rawPeriodEmployeeSetting.Employees.Count() > 0)
                {
                    periodEmployeeSettings = ConvertToPeriodEmployeeSetting(rawPeriodEmployeeSetting, startDate);
                }
            }

            return periodEmployeeSettings;
        }

        public CommandResult AddEmployeeToPeriod(AddEmployeeToPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        private static IEnumerable<PeriodEmployeeSettingModel> ConvertToPeriodEmployeeSetting(
            RawPeriodEmployeeSettingModel rawPeriodEmployeeSetting,
            string startDate)
        {
            IEnumerable<PeriodEmployeeSettingModel> employeeSettings = rawPeriodEmployeeSetting.Employees;

            foreach (PeriodEmployeeSettingModel employeeSetting in employeeSettings)
            {
                GetFlowSetting(rawPeriodEmployeeSetting.FlowSettings, employeeSetting, startDate);
            }

            return employeeSettings;
        }

        private static void GetFlowSetting(
            IEnumerable<RawPeriodFlowSettingModel> rawFlowSettings,
            PeriodEmployeeSettingModel employeeSetting,
            string startDate)
        {
            if (rawFlowSettings != null)
            {
                IEnumerable<RawPeriodFlowSettingModel> rawEmployeeFlowSettings =
                           rawFlowSettings.Where(x => x.PeriodId == employeeSetting.PeriodId && x.EmployeeId == employeeSetting.EmployeeId);

                foreach (RawPeriodFlowSettingModel rawFlowSetting in rawEmployeeFlowSettings)
                {
                    employeeSetting = GetEvaluators(rawFlowSetting, employeeSetting);
                }

                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    employeeSetting = CalculateFlowDate(employeeSetting, startDate);
                }
            }
        }

        private static PeriodEmployeeSettingModel CalculateFlowDate(PeriodEmployeeSettingModel employeeSetting, string startDate)
        {
            DateTime periodStartDate = Convert.ToDateTime(startDate);

            int duration = employeeSetting.HumanResources.Count() > 0 ? employeeSetting.HumanResources[0].Duration : 0;
            int startDuration = 0,
                endDuration = duration > 0 ? duration - 1 : duration;

            IList<PeriodFlowModel> selfEvaluators = employeeSetting.SelfEvaluators;
            IList<PeriodFlowModel> evaluators = employeeSetting.Evaluators;
            IList<PeriodFlowModel> reviewers = employeeSetting.Reviewers;
            IList<PeriodFlowModel> managements = employeeSetting.Managements;

            if (selfEvaluators.Count() > 0)
            {
                duration = employeeSetting.SelfEvaluators[0].Duration;
                startDuration = duration > 0 ? endDuration + 1 : endDuration;
                endDuration = duration > 0 ? startDuration + duration - 1 : startDuration;
                CalculateEvaluatorDate(employeeSetting.SelfEvaluators, periodStartDate, startDuration, endDuration);
            }

            if (evaluators.Count() > 0 && evaluators[0].EvaluatorInfo != null && evaluators[0].EvaluatorInfo.EmployeeId != -1)
            {
                duration = employeeSetting.Evaluators[0].Duration;
                startDuration = duration > 0 ? endDuration + 1 : endDuration;
                endDuration = duration > 0 ? startDuration + duration - 1 : startDuration;
                CalculateEvaluatorDate(employeeSetting.Evaluators, periodStartDate, startDuration, endDuration);
            }

            if (reviewers.Count() > 0 && reviewers[0].EvaluatorInfo != null && reviewers[0].EvaluatorInfo.EmployeeId != -1)
            {
                duration = employeeSetting.Reviewers[0].Duration;
                startDuration = duration > 0 ? endDuration + 1 : endDuration;
                endDuration = duration > 0 ? startDuration + duration - 1 : startDuration;
                CalculateEvaluatorDate(employeeSetting.Reviewers, periodStartDate, startDuration, endDuration);
            }

            if (managements.Count() > 0 && managements[0].EvaluatorInfo != null && managements[0].EvaluatorInfo.EmployeeId != -1) 
            {
                duration = employeeSetting.Managements[0].Duration;
                startDuration = duration > 0 ? endDuration + 1 : endDuration;
                endDuration = duration > 0 ? startDuration + duration - 1 : startDuration;
                CalculateEvaluatorDate(employeeSetting.Managements, periodStartDate, startDuration, endDuration);
            }

            return employeeSetting;
        }

        private static void CalculateEvaluatorDate(
            IList<PeriodFlowModel> evaluators,
            DateTime periodStartDate,
            int startDuration,
            int endDuration)
        {
            foreach (PeriodFlowModel evaluator in evaluators)
            {
                evaluator.StartDate = periodStartDate.AddDays(startDuration);
                evaluator.EndDate = periodStartDate.AddDays(endDuration);
            }
        }

        private static PeriodEmployeeSettingModel GetEvaluators(
            RawPeriodFlowSettingModel rawFlowSetting,
            PeriodEmployeeSettingModel employeeSetting)
        {
            switch (rawFlowSetting.ProcessId)
            {
                case (int)ProcessType.HumanResource:
                    PeriodFlowModel humanResources = CreateEvaluator(rawFlowSetting);
                    employeeSetting.HumanResources.Add(humanResources);
                    break;

                case (int)ProcessType.Self:
                    PeriodFlowModel selfEvaluator = CreateEvaluator(rawFlowSetting);
                    employeeSetting.SelfEvaluators.Add(selfEvaluator);
                    break;

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
