// ----------------------------------------------------------------------
// <copyright file="AddEmployeeToPeriodCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class AddEmployeeToPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string Departments { get; set; }

        public int? EmployeeId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_AddEmployee";
        }
    }
}
