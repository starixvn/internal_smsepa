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
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.FlowManagement;

    public interface IFlowManagementService
    {
        #region Flow Template

        IEnumerable<FlowTemplateModel> GetFlowTemplate(GetFlowTemplateCriteria criteria);

        FlowTemplateModel GetFlowTemplateById(GetFlowTemplateByIdCriteria criteria);

        CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria);

        CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria);

        #endregion Flow Template

        #region Department Flow Setting

        IEnumerable<FlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria);

        CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria);

        CommandResult DeleteDepartmentFlow(DeleteDepartmentFlowCriteria criteria);

        #endregion Department Flow Setting

        #region Employee Flow Setting

        IEnumerable<FlowModel> GetPersonalFlow(GetPersonalFlowCriteria criteria);

        CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria);

        CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria);

        #endregion Employee Flow Setting
    }
}