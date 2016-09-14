namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using System.Collections.Generic;

    public interface IFlowSettingService
    {
        #region Department Flow Setting

        IEnumerable<DepartmentFlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria);

        IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria);

        CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria);

        CommandResult DeleteDepartmentFlow(DeleteDepartmentFlowCriteria criteria);

        IList<Department> GetDepartmentHasSetting(GetDepartmentSectionHasSettingCriteria criteria);

        #endregion Department Flow Setting

        #region Employee Flow Setting

        IEnumerable<PersonalFlowModel> GetPersonalFlow(GetPersonalFlowCriteria criteria);

        IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria);

        CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria);

        CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria);

        IEnumerable<Employee> GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria);

        #endregion Employee Flow Setting
    }
}