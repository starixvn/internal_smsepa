// ----------------------------------------------------------------------
// <copyright file="RawEmployeeRoleModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 4:25:35 PM">Created.</change>
// </history>

namespace SMS.EPA.Model.Employee
{
    /// <summary>
    /// The raw employee role model.
    /// </summary>
    public class RawEmployeeRoleModel
    {
        public int EmployeeId { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}
