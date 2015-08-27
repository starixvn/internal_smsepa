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
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;

    public class GenerationEvaluationService : BaseService
    {
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
    }
}
