namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

    public interface IPersonalFlowService
    {
        IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(int flowTemplateId);

        IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria);

        CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria);

        CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria);

        EmployeeFlowSettingModel GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria);
    }
}