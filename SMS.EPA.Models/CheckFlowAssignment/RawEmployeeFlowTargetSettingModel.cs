// ----------------------------------------------------------------------
// <copyright file="RawEmployeeFlowTargetSettingModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/24/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.CheckFlowAssignment
{
    using System.Collections.Generic;
    using SMS.EPA.Model.TargetEmployeeSetting;

    public class RawEmployeeFlowTargetSettingModel
    {
        public IEnumerable<EmployeeFlowTargetSettingModel> Employees { get; set; }

        public IEnumerable<TargetEmployeeModel> TargetEmployees { get; set; }

        public IEnumerable<RawEmployeeFlowModel> EmployeeFlows { get; set; }
    }
}
