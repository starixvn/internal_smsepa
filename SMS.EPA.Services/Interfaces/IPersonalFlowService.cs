namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IPersonalFlowService
    {
        IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(int flowTemplateId);

        IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria);
    }
}