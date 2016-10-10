namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;

    using SMS.EPA.Services.Interfaces;

    public class PersonalFlowService : BaseService, IPersonalFlowService
    {
        private readonly IFlowSettingRepository flowSettingRepository;

        public PersonalFlowService()
            : this(null)
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
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = this.Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = this.Repository.Delete(criteria);
            }

            return result;
        }

        public EmployeeFlowSettingModel GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria)
        {
            var result = new EmployeeFlowSettingModel();

            if (criteria != null && criteria.IsValid())
            {
                IEnumerable<EvaluatorInfoViewModel> data = this.GetData<EvaluatorInfoViewModel>(criteria).OrderBy(i => i.NickName);
                result.DefaultFlowSettings = data.Where(x => x.IsExistsSetting == false);
                result.PersonalFlowSettings = data.Where(x => x.IsExistsSetting == true);
            }

            return result;
        }
    }
}