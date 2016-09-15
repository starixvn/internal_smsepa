namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IPersonalFlowService
    {
        IEnumerable<PersonalFlowModel> GetPersonalFlow(GetPersonalFlowCriteria criteria);

        IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria);

        CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria);

        CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria);

        EmployeeFlowSettingModel GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria);
    }
}