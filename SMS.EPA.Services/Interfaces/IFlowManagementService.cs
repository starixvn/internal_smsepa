namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.FlowManagement;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.FlowManagement;

    public interface IFlowManagementService
    {
        #region Flow Template

        IEnumerable<FlowTemplateModel> GetFlowTemplate(GetFlowTemplateCriteria criteria);

        FlowTemplateModel GetFlowTemplateById(GetFlowTemplateByIdCriteria criteria);

        CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria);

        CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria);

        CommandResult UpdateStatusFlowTemplate(UpdateStatusFlowTemplateCriteria criteria);

        CommandResult DeleteFlowTemplate(DeleteFlowTemplateCriteria criteria);

        #endregion Flow Template

        #region Department Flow Setting

        IEnumerable<DepartmentFlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria);

        IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria);

        CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria);

        CommandResult DeleteDepartmentFlow(DeleteDepartmentFlowCriteria criteria);

        IList<DepartmentModel> GetDepartmentHasSetting(GetDepartmentSectionHasSettingCriteria criteria);

        #endregion Department Flow Setting

        #region Employee Flow Setting

        IEnumerable<PersonalFlowModel> GetPersonalFlow(GetPersonalFlowCriteria criteria);

        IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria);

        CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria);

        CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria);

        #endregion Employee Flow Setting
    }
}