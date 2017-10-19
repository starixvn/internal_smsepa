namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;
    using Model.Enitites;

    public interface IDepartmentFlowService
    {
        IEnumerable<DepartmentFlowModel> GetDepartmentFlowByFlowTemplateId(int flowTemplateId);

        IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria);

        IList<Department> GetDepartmentHasSettingByFlowTemplateId(int flowTemplateId);
    }
}