// ----------------------------------------------------------------------
// <copyright file="AssignFunctionToRoleCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 1:45:44 PM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.RoleManagement
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Assign function to role criteria.
    /// </summary>
    public class AssignFunctionToRoleCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public string Functions { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return RoleId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaRole_AssignFunctionToRole";
        }
    }
}
