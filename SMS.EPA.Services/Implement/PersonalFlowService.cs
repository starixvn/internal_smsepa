namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    public class PersonalFlowService : BaseService, IPersonalFlowService
    {
        private readonly IFlowSettingRepository flowSettingRepository;

        public PersonalFlowService() : this(null)
        {
        }

        public PersonalFlowService(IFlowSettingRepository flowSettingRepository)
        {
            this.flowSettingRepository = flowSettingRepository ?? new FlowSettingRepository();
        }

        public IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(int flowTemplateId)
        {
            var criteria = new GetPersonalFlowByFlowTemplateIdCriteria
            {
                FlowTemplateId = flowTemplateId
            };
            IEnumerable<PersonalFlowModel> flows = new List<PersonalFlowModel>();

            if (criteria.IsValid())
            {
                flows = flowSettingRepository.GetPersonalFlowByFlowTemplateId(criteria).OrderBy(i => i.Group.GroupName);
            }

            return flows;
        }

        public IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria)
        {
            IEnumerable<FlowModel> flow = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flow = flowSettingRepository.GetFlowSettingByFlowTemplateId(criteria);
            }

            return flow;
        }

        public CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public EmployeeFlowSettingModel GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria)
        {
            var result = new EmployeeFlowSettingModel();

            if (criteria != null && criteria.IsValid())
            {
                var data = GetData<EvaluatorInfoViewModel>(criteria).OrderBy(i => i.NickName);
                result.DefaultFlowSettings = data.Where(x => !x.IsExistsSetting);
                result.PersonalFlowSettings = data.Where(x => x.IsExistsSetting);
            }

            return result;
        }
    }
}