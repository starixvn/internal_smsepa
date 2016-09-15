namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;

    public interface IDepartmentFlowService
    {
        IEnumerable<DepartmentFlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria);

        IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria);

        CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria);

        CommandResult DeleteDepartmentFlow(DeleteDepartmentFlowCriteria criteria);

        IList<Department> GetDepartmentHasSetting(GetDepartmentSectionHasSettingCriteria criteria);
    }
}