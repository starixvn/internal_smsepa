// ----------------------------------------------------------------------
// <copyright file="Employee.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 10:44:42 AM">Created.</change>
// </history>

namespace SMS.EPA.Model.Enitites
{
    /// <summary>
    /// The employee model.
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string DomainUsername { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Password { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }
    }
}
