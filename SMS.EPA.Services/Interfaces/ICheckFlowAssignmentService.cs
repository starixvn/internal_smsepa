// ----------------------------------------------------------------------
// <copyright file="ICheckFlowAssignmentService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/24/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.CheckFlowAssignment;
    using SMS.EPA.Model.CheckFlowAssignment;

    public interface ICheckFlowAssignmentService
    {
        IEnumerable<EmployeeFlowTargetSettingModel> GetEmployeeFlowTargetSetting(GetEmployeeFlowSettingCriteria criteria);

        CommandResult UpdateEmployeeFlowTargetSetting(UpdateEmployeeFlowSettingCriteria criteria);
    }
}
