// ----------------------------------------------------------------------
// <copyright file="InsertEmployeeToProbationPeriodCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/01/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using Fanex.Data.Repository;

    public class InsertEmployeeToProbationPeriodCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }

        public string EmployeeIds { get; set; }

        public override bool IsValid()
        {
            return PeriodId != null && PeriodId > 0 && !string.IsNullOrWhiteSpace(EmployeeIds);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_Probation_AddEmployeeToPeriod";
        }
    }
}
