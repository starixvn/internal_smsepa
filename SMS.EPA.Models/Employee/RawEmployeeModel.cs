// ----------------------------------------------------------------------
// <copyright file="RawEmployeeModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 4:22:50 PM">Created.</change>
// </history>

namespace SMS.EPA.Model.Employee
{
    using System.Collections.Generic;

    /// <summary>
    /// The raw employee model.
    /// </summary>
    public class RawEmployeeModel
    {
        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<RawEmployeeRoleModel> Roles { get; set; }
    }
}
