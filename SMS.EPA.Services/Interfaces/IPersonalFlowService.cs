namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;

    public interface IPersonalFlowService
    {
        IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(int flowTemplateId);

        IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria);

        EmployeeFlowSettingModel GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria);
    }
}