namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface IFlowSettingRepository
    {
        IEnumerable<FlowModel> GetFlowManagement(ICriteria criteria);

        IEnumerable<DepartmentFlowModel> GetDepartmentFlow(ICriteria criteria);

        IEnumerable<PersonalFlowModel> GetPersonalFlow(ICriteria criteria);
    }
}