namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IDepartmentFlowService
    {
        IEnumerable<DepartmentFlowModel> GetByFlowTemplateId(int flowTemplateId);

        IEnumerable<FlowModel> GetById(GetDepartmentFlowByIdCriteria criteria);
    }
}