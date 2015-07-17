// ----------------------------------------------------------------------
// <copyright file="RawTargetEmployeeSettingModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/17/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;

    public class RawTargetEmployeeSettingModel
    {
        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<TargetEmployeeModel> TargetEmployees { get; set; }
    }
}
