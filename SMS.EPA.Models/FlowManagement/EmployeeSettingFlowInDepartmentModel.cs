// -----------------------------------------------------------------------
// <copyright file="EmployeeSettingFlowInDepartmentModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.22">Created</change>
// </history>

namespace SMS.EPA.Model.FlowManagement
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;

    public class EmployeeSettingFlowInDepartmentModel
    {
        public IEnumerable<EmployeeModel> PersonalFlowSettingEmployees { get; set; }

        public IEnumerable<EmployeeModel> DefaultFlowSettingEmployees { get; set; }
    }
}