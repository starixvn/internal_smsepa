// ----------------------------------------------------------------------
// <copyright file="GetEmployeeFlowSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/24/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.CheckFlowAssignment
{
    using Fanex.Data.Repository;

    public class UpdateEmployeeFlowSettingCriteria : CriteriaBase
    {
        public string EmployeeAssignments { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployeeAssignmentSetting_Edit";
        }
    }
}
