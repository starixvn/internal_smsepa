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
        IEnumerable<FlowTemplateModel> GetFlowTemplate(GetFlowTemplateCriteria criteria);

        IEnumerable<FlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria);

        IEnumerable<FlowModel> GetPersonalFlow(GetPersonalFlowCriteria criteria);

        CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria);

        CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria);

        CommandResult DeleteDepartmentFlow(DeleteDepartmentFlowCriteria criteria);

        CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria);
    }
}