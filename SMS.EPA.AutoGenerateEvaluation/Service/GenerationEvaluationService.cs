// ----------------------------------------------------------------------
// <copyright file="GenerationEvaluationService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/26/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Service
{
    using System.Collections.Generic;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.AutoGenerateEvaluation.Repository;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.Period;

    public class GenerationEvaluationService : BaseService
    {
        private readonly GenerateEvaluationRepository _generateEvaluationRepository;

        public GenerationEvaluationService()
            : this(null)
        {
        }

        public GenerationEvaluationService(GenerateEvaluationRepository generationEvaluationRepository)
        {
            _generateEvaluationRepository = generationEvaluationRepository ?? new GenerateEvaluationRepository();
        }

        public IEnumerable<int> CheckStartSemiAnnual()
        {
            var criteria = new GetSemiAnnualPeriodStartedCriteria();
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        public CommandResult InsertEmployeeToSemiAnnualPeriod(int periodId)
        {
            var commandResult = new CommandResult();
            var criteria = new InsertEmployeeToSemiAnnualPeriodCriteria
            {
                PeriodId = periodId
            };

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult AutoGenerateEvaluation(int periodId)
        {
            var commandResult = new CommandResult();
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = periodId
            };

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(int periodId)
        {
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId
            };
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            if (criteria.IsValid())
            {
                employeeSettings = _generateEvaluationRepository.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }
    }
}