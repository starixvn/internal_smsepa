// ----------------------------------------------------------------------
// <copyright file="EmployeeModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 10:51:11 AM">Created.</change>
// </history>

namespace SMS.EPA.Model.Employee
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.RoleManagement;

    [Serializable]
    public class EmployeeModel : Employee
    {
        public EmployeeModel()
        {
            Roles = new List<SystemRoleModel>();
        }

        public string Username
        {
            get
            {
                return !string.IsNullOrWhiteSpace(DomainUsername) && IsSpecialUser ? DomainUsername.Replace("sms\\", string.Empty) : string.Empty;
            }
        }

        public bool IsSpecialUser { get; set; }

        public bool IsExistsSetting { get; set; }

        public JobTitle JobTitle { get; set; }

        public DepartmentModel Department { get; set; }

        public Group Group { get; set; }

        public IList<SystemRoleModel> Roles { get; set; }

        public DateTime JoinedDate { get; set; }

        public bool HasMonthlyEPA { get; set; }
    }
}