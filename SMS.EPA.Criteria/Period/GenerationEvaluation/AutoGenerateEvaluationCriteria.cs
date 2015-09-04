// ----------------------------------------------------------------------
// <copyright file="AutoGenerateEvaluationCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/04/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period.GenerationEvaluation
{
    using Fanex.Data.Repository;

    public class AutoGenerateEvaluationCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string EmployeeIds { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GenerateEPA";
        }
    }
}
