namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public interface IFlowSettingRepository
    {
        IEnumerable<FlowModel> GetFlowSettingByFlowTemplateId(ICriteria criteria);

        IEnumerable<DepartmentFlowModel> GetDepartmentFlowByFlowTemplateId(ICriteria criteria);

        IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(ICriteria criteria);
    }
}