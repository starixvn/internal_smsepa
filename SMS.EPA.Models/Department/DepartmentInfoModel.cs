// -----------------------------------------------------------------------
// <copyright file="DepartmentInfoModel.cs"  company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.21">Created</change>
// </history>

namespace SMS.EPA.Model.Department
{
    using System.Collections.Generic;

    public class DepartmentInfoModel
    {
        public DepartmentInfoModel()
        {
            Departments = new List<DepartmentModel>();
        }

        public DepartmentModel TreeDepartment { get; set; }

        public IList<DepartmentModel> Departments { get; set; }
    }
}