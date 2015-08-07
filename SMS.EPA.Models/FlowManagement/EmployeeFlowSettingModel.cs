﻿// -----------------------------------------------------------------------
// <copyright file="EmployeeFlowSettingModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.07">Created</change>
// </history>

namespace SMS.EPA.Model.FlowManagement
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;

    public class EmployeeFlowSettingModel
    {
        public IEnumerable<EmployeeModel> DefaultFlowSettings { get; set; }

        public IEnumerable<EmployeeModel> PersonalFlowSettings { get; set; }
    }
}