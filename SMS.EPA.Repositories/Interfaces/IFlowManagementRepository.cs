namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.FlowManagement;

    public interface IFlowManagementRepository
    {
        IEnumerable<FlowTemplateModel> GetFlowTemplate(ICriteria criteria);

        FlowTemplateModel GetFlowTemplateById(ICriteria criteria);

        IEnumerable<FlowModel> GetFlowManagement(ICriteria criteria);

        IEnumerable<DepartmentFlowModel> GetDepartmentFlow(ICriteria criteria);

        IEnumerable<PersonalFlowModel> GetPersonalFlow(ICriteria criteria);
    }
}