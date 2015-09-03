// ----------------------------------------------------------------------
// <copyright file="GenerationEvaluationService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/26/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Service.Implement
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.AutoGenerateEvaluation.Model;
    using SMS.EPA.AutoGenerateEvaluation.Repository;
    using SMS.EPA.AutoGenerateEvaluation.Service.Interfaces;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.Period;

    public class GenerationEvaluationService : BaseService, IGenerationEvaluationService
    {
        private readonly IGenerationEvaluationRepository _generateEvaluationRepository;

        public GenerationEvaluationService()
            : this(null)
        {
        }

        public GenerationEvaluationService(IGenerationEvaluationRepository generationEvaluationRepository)
        {
            _generateEvaluationRepository = generationEvaluationRepository ?? new GenerationEvaluationRepository();
        }

        public IEnumerable<int> CheckStartSemiAnnual(GetSemiAnnualPeriodStartedCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        public CommandResult InsertEmployeeToSemiAnnualPeriod(InsertEmployeeToSemiAnnualPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult AutoGenerateEvaluation(AutoGenerateEvaluationCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(GetEmployeeSettingByPeriodIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            if (criteria.IsValid())
            {
                employeeSettings = _generateEvaluationRepository.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }

        public AutoEmployeePeriodModel GetEmployeeProbationEvaluation(GetEmployeeProbationEvaluationCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var employeePeriod = new AutoEmployeePeriodModel();

            if (criteria.IsValid())
            {
                employeePeriod = _generateEvaluationRepository.GetEmployeeProbationEvaluation(criteria);
            }

            return employeePeriod;
        }

        public CommandResult InsertEmployeeToProbationPeriod(InsertEmployeeToProbationPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var comandResult = new CommandResult();

            if (criteria.IsValid())
            {
                comandResult = Repository.Insert(criteria);
            }

            return comandResult;
        }

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasStartedProcess(GetPeriodHasStartedProcessCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<AutoEmployeePeriodModel> startedPeriods = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return startedPeriods;
        }

        public bool CheckPeriodStartedSendMailAll(CheckPeriodStartedSendMailAllCriteria criteria)
        {

            Check.Argument.IsNotNull(criteria, "criteria");
            bool hasSendMailAll = false;

            if (criteria.IsValid())
            {
                hasSendMailAll = GetDataEntity<bool>(criteria);
            }

            return hasSendMailAll;
        }

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess(GetPeriodHasReminderProcessCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<AutoEmployeePeriodModel> reminderPeriods = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                reminderPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return reminderPeriods;
        }
    }
}