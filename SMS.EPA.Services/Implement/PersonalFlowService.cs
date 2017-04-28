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
        private readonly IFlowSettingRepository _flowSettingRepository;

        public PersonalFlowService()
            : this(null)
        {
        }

        public PersonalFlowService(IFlowSettingRepository flowSettingRepository)
        {
            _flowSettingRepository = flowSettingRepository ?? new FlowSettingRepository();
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
                flows = _flowSettingRepository.GetPersonalFlowByFlowTemplateId(criteria).OrderBy(i => i.Group.GroupName);
            }

            return flows;
        }

        public IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria)
        {
            IEnumerable<FlowModel> flow = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flow = _flowSettingRepository.GetFlowSettingByFlowTemplateId(criteria);
            }

            return flow;
        }

        public CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }

        public EmployeeFlowSettingModel GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria)
        {
            var result = new EmployeeFlowSettingModel();

            if (criteria != null && criteria.IsValid())
            {
                IEnumerable<EvaluatorInfoViewModel> data = GetData<EvaluatorInfoViewModel>(criteria).OrderBy(i => i.NickName);
                result.DefaultFlowSettings = data.Where(x => !x.IsExistsSetting);
                result.PersonalFlowSettings = data.Where(x => x.IsExistsSetting);
            }

            return result;
        }
    }
}