// -----------------------------------------------------------------------
// <copyright file="FlowManagementService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.17">Created</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.FlowManagement;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class FlowManagementService : BaseService, IFlowManagementService
    {
        private readonly IFlowManagementRepository _flowManagementRepository;

        public FlowManagementService()
            : this(null)
        {
        }

        public FlowManagementService(IFlowManagementRepository flowManagementRepository)
        {
            _flowManagementRepository = flowManagementRepository ?? new FlowManagementRepository();
        }

        #region Flow Template

        public IEnumerable<FlowTemplateModel> GetFlowTemplate(GetFlowTemplateCriteria criteria)
        {
            IEnumerable<FlowTemplateModel> flowTemplates = new List<FlowTemplateModel>();

            if (criteria != null && criteria.IsValid())
            {
                flowTemplates = _flowManagementRepository.GetFlowTemplate(criteria);
            }

            return flowTemplates;
        }

        public FlowTemplateModel GetFlowTemplateById(GetFlowTemplateByIdCriteria criteria)
        {
            FlowTemplateModel flowTemplate = new FlowTemplateModel();

            if (criteria != null && criteria.IsValid())
            {
                flowTemplate = _flowManagementRepository.GetFlowTemplateById(criteria);
            }

            return flowTemplate;
        }

        public CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult UpdateStatusFlowTemplate(UpdateStatusFlowTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult DeleteFlowTemplate(DeleteFlowTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }

        #endregion Flow Template

        #region Department Flow Setting

        public IEnumerable<DepartmentFlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria)
        {
            IEnumerable<DepartmentFlowModel> flows = new List<DepartmentFlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flows = _flowManagementRepository.GetDepartmentFlow(criteria);
                flows = flows.OrderBy(i => i.Name);
            }

            return flows;
        }

        public IEnumerable<FlowModel> GetDepartmentFlowById(GetDepartmentFlowByIdCriteria criteria)
        {
            IEnumerable<FlowModel> flow = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flow = _flowManagementRepository.GetFlowManagement(criteria);
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
                flows = _flowManagementRepository.GetPersonalFlow(criteria);
            }

            return flows;
        }

        public IEnumerable<FlowModel> GetPersonalFlowById(GetPersonalFlowByIdCriteria criteria)
        {
            IEnumerable<FlowModel> flow = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                flow = _flowManagementRepository.GetFlowManagement(criteria);
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