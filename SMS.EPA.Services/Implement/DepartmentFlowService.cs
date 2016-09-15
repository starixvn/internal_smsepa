namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class DepartmentFlowService : BaseService, IDepartmentFlowService
    {
        private readonly IFlowSettingRepository flowSettingRepository;

        public DepartmentFlowService()
            : this(null)
        {
        }

        public DepartmentFlowService(IFlowSettingRepository flowSettingRepository)
        {
            this.flowSettingRepository = flowSettingRepository ?? new FlowSettingRepository();
        }

        public IEnumerable<DepartmentFlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria)
        {
            IEnumerable<DepartmentFlowModel> flows = new List<DepartmentFlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flows = flowSettingRepository.GetDepartmentFlow(criteria).OrderBy(i => i.Name);
            }

            return flows;
        }

        public IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria)
        {
            IEnumerable<FlowModel> flow = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flow = flowSettingRepository.GetFlowManagement(criteria);
            }

            return flow;
        }

        public CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = this.Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult DeleteDepartmentFlow(DeleteDepartmentFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = this.Repository.Delete(criteria);
            }

            return result;
        }

        public IList<Department> GetDepartmentHasSetting(GetDepartmentSectionHasSettingCriteria criteria)
        {
            IList<Department> departments = new List<Department>();

            if (criteria != null && criteria.IsValid())
            {
                departments = this.GetData<Department>(criteria).ToList();
            }

            return departments;
        }
    }
}