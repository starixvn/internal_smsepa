// ----------------------------------------------------------------------
// <copyright file="GenerationEvaluationRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/31/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Period;

    public class GenerationEvaluationRepository : IGenerationEvaluationRepository
    {
        public AutoEmployeePeriodModel GetEmployeeProbationEvaluation(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<int> employees = multi.Read<int>();
                    IEnumerable<int> periods = multi.Read<int>();
                    var employeePeriod = new AutoEmployeePeriodModel();

                    if (periods != null && employees != null)
                    {
                        employeePeriod.PeriodId = periods.First();
                        employeePeriod.EmployeeIds = string.Join(",", employees);
                    }

                    return employeePeriod;
                }
            }
        }
    }
}
