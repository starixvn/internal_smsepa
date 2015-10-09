// -----------------------------------------------------------------------
// <copyright file="EPAEmployeeModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.15">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Enitites;

    public class EPAEmployeeModel
    {
        public int EPAId { get; set; }

        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Avatar { get; set; }

        public Group Group { get; set; }

        public DepartmentModel Department { get; set; }

        public JobTitle JobTitle { get; set; }
    }
}