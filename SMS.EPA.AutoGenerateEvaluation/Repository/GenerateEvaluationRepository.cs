// ----------------------------------------------------------------------
// <copyright file="GenerateEvaluationRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/31/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Repository
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Period;

    public class GenerateEvaluationRepository
    {
        public IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<PeriodFlowModel, EmployeeModel, PeriodFlowModel>(
                    (empS, eva) =>
                    {
                        empS.EvaluatorInfo = eva;
                        return empS;
                    },
                    "EvaluatorId",
                    criteria.GetSpParams());
            }
        }
    }
}
