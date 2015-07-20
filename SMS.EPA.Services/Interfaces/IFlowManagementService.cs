// -----------------------------------------------------------------------
// <copyright file="IFlowManagementService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.17">Created</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.FlowManagement;
    using SMS.EPA.Model.FlowManagement;

    public interface IFlowManagementService
    {
        IEnumerable<EPAFlowModel> GetFlowDepartment(GetDepartmentFlowCriteria criteria);

        IEnumerable<EPAFlowModel> GetFlowGroup(GetGroupFlowCriteria criteria);

        CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria);

        CommandResult CreateGroupFlow(CreateGroupFlowCriteria criteria);
    }
}