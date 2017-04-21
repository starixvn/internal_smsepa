namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;
    using Model.Enitites;

    public interface IDepartmentFlowService
    {
        IEnumerable<DepartmentFlowModel> GetDepartmentFlowByFlowTemplateId(int flowTemplateId);

        IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria);

        CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria);

        CommandResult DeleteDepartmentFlow(DeleteDepartmentFlowCriteria criteria);

        IList<Department> GetDepartmentHasSettingByFlowTemplateId(int flowTemplateId);
    }
}