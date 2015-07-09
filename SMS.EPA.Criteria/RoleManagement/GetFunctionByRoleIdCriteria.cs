// ----------------------------------------------------------------------
// <copyright file="GetFunctionByRoleIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 1:44:04 PM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.RoleManagement
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get function by role ID criteria.
    /// </summary>
    public class GetFunctionByRoleIdCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public override bool IsValid()
        {
            return RoleId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaRole_GetFunctionByRoleId";
        }
    }
}
