namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface IFlowSettingRepository
    {
        IEnumerable<FlowModel> GetFlowSettingByFlowTemplateId(ICriteria criteria);

        IEnumerable<DepartmentFlowModel> GetDepartmentFlowByFlowTemplateId(ICriteria criteria);

        IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(ICriteria criteria);
    }
}