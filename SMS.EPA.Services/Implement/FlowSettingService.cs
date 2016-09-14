namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class FlowSettingService : BaseService, IFlowSettingService
    {
        private readonly IFlowSettingRepository flowSettingRepository;

        public FlowSettingService()
            : this(null)
        {
        }

        public FlowSettingService(IFlowSettingRepository flowSettingRepository)
        {
            this.flowSettingRepository = flowSettingRepository ?? new FlowSettingRepository();
        }

        #region Department Flow Setting

        public IEnumerable<DepartmentFlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria)
        {
            IEnumerable<DepartmentFlowModel> flows = new List<DepartmentFlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flows = flowSettingRepository.GetDepartmentFlow(criteria);
                flows = flows.OrderBy(i => i.Name);
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

        public IList<DepartmentModel> GetDepartmentHasSetting(GetDepartmentSectionHasSettingCriteria criteria)
        {
            IList<DepartmentModel> departments = new List<DepartmentModel>();

            if (criteria != null && criteria.IsValid())
            {
                departments = GetData<DepartmentModel>(criteria).ToList();
            }

            return departments;
        }

        #endregion Department Flow Setting

        #region Employee Flow Setting

        public IEnumerable<PersonalFlowModel> GetPersonalFlow(GetPersonalFlowCriteria criteria)
        {
            IEnumerable<PersonalFlowModel> flows = new List<PersonalFlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flows = flowSettingRepository.GetPersonalFlow(criteria);
                flows = flows.OrderBy(i => i.Group.GroupName);
            }

            return flows;
        }

        public IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria)
        {
            IEnumerable<FlowModel> flow = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flow = flowSettingRepository.GetFlowManagement(criteria);
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

        public IEnumerable<EmployeeModel> GetEmployeeFlowSetting(GetEmployeeFlowSettingCriteria criteria)
        {
            IEnumerable<EmployeeModel> data = new List<EmployeeModel>();

            if (criteria != null && criteria.IsValid())
            {
                data = GetData<EmployeeModel>(criteria).OrderBy(i => i.NickName);
            }

            return data;
        }

        #endregion Employee Flow Setting
    }
}