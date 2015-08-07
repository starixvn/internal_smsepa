// ----------------------------------------------------------------------
// <copyright file="RawTargetDepartmentSettingModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/7/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Department;

    public class RawTargetDepartmentSettingModel
    {
        public IEnumerable<DepartmentModel> Departments { get; set; }

        public IEnumerable<TargetDepartmentModel> TargetDepartments { get; set; }
    }
}
