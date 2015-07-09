// ----------------------------------------------------------------------
// <copyright file="AssignRoleToEmployeeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 3:16:08 PM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Assign role to employee criteria.
    /// </summary>
    public class AssignRoleToEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string RoleIds { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_AssignRoleToEmployee";
        }
    }
}
