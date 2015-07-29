// ----------------------------------------------------------------------
// <copyright file="GetDepartmentFlowSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/29/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.CheckFlowAssignment
{
    using Fanex.Data.Repository;

    public class GetDepartmentFlowSettingCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaDepartmentAssignmentSetting_Get";
        }
    }
}
