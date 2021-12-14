namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IPersonalFlowService
    {
        IEnumerable<PersonalFlowModel> GetByFlowTemplateId(int flowTemplateId);

        IEnumerable<FlowModel> GetById(GetPersonalFlowByIdCriteria criteria);
    }
}