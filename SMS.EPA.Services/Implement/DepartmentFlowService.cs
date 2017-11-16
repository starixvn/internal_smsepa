namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;

    public class DepartmentFlowService : IDepartmentFlowService
    {
        private readonly IFlowSettingRepository flowSettingRepository;

        public DepartmentFlowService() : this(null)
        {
        }

        public DepartmentFlowService(IFlowSettingRepository flowSettingRepository)
        {
            this.flowSettingRepository = flowSettingRepository ?? new FlowSettingRepository();
        }

        public IEnumerable<DepartmentFlowModel> GetDepartmentFlowByFlowTemplateId(int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowByFlowTemplateIdCriteria
            {
                FlowTemplateId = flowTemplateId
            };
            IEnumerable<DepartmentFlowModel> flows = new List<DepartmentFlowModel>();

            if (criteria.IsValid())
            {
                flows = flowSettingRepository.GetDepartmentFlowByFlowTemplateId(criteria).OrderBy(i => i.Name);
            }

            return flows;
        }

        public IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria)
        {
            IEnumerable<FlowModel> flow = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flow = flowSettingRepository.GetFlowSettingByFlowTemplateId(criteria);
            }

            return flow;
        }
    }
}