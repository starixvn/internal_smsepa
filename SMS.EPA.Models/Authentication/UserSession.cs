// ----------------------------------------------------------------------
// <copyright file="UserSession.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 10:37:01 AM">Created.</change>
// </history>

namespace SMS.EPA.Model.Authentication
{
    using System.Collections.Generic;
    using SMS.EPA.Model.EmployeeManagement;
    using SMS.EPA.Model.RoleManagement;

    /// <summary>
    /// The user session model.
    /// </summary>
    public class UserSession
    {
        public UserSession()
        {
            Roles = new List<SystemRoleModel>();
        }

        public EmployeeModel EmployeeInfo { get; set; }

        public IEnumerable<SystemRoleModel> Roles { get; set; }
    }
}
