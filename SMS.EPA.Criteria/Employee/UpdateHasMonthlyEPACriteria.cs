// -----------------------------------------------------------------------
// <copyright file="UpdateHasMonthlyEPACriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.01.29">Created</change>
// </history>

namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class UpdateHasMonthlyEPACriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EpaEmployee_UpdateHasMonthlyEPA";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}