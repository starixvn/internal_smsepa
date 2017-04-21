namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Model.Enitites;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    public class DepartmentFlowService : BaseService, IDepartmentFlowService
    {
        private readonly IFlowSettingRepository _flowSettingRepository;

        public DepartmentFlowService()
            : this(null)
        {
        }

        public DepartmentFlowService(IFlowSettingRepository flowSettingRepository)
        {
            _flowSettingRepository = flowSettingRepository ?? new FlowSettingRepository();
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
                flows = _flowSettingRepository.GetDepartmentFlowByFlowTemplateId(criteria).OrderBy(i => i.Name);
            }

            return flows;
        }

        public IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria)
        {
            IEnumerable<FlowModel> flow = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flow = _flowSettingRepository.GetFlowSettingByFlowTemplateId(criteria);
            }

            return flow;
        }

        public CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult DeleteDepartmentFlow(DeleteDepartmentFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }

        public IList<Department> GetDepartmentHasSettingByFlowTemplateId(int flowTemplateId)
        {
            var criteria = new GetDepartmentSectionHasSettingCriteria
            {
                FlowTemplateId = flowTemplateId
            };
            IList<Department> departments = GetData<Department>(criteria).ToList();

            return departments;
        }
    }
}