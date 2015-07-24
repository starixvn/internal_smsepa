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

    public class GetEmployeeFlowSettingCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public int? TargetTypeId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployeeAssignmentSetting_Get";
        }
    }
}
