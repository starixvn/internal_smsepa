// -----------------------------------------------------------------------
// <copyright file="DepartmentModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.21">Created</change>
// </history>

namespace SMS.EPA.Model.Department
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;

    public class DepartmentModel
    {
        public DepartmentModel()
        {
            Childrens = new List<DepartmentModel>();
        }

        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsGroup { get; set; }

        public int DepartmentLevel { get; set; }

        public DateTime? CreatedDate { get; set; }

        public EmployeeModel Creator { get; set; }

        public EmployeeModel Manager { get; set; }

        public EmployeeModel Modifier { get; set; }

        public List<DepartmentModel> Childrens { get; set; }
    }
}