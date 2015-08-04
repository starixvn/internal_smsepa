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
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.FlowManagement;
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

        public IEnumerable<FlowTemplateModel> GetFlowTemplate(GetFlowTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<FlowTemplateModel> flowTemplates = new List<FlowTemplateModel>();
            if (criteria.IsValid())
            {
                flowTemplates = _flowManagementRepository.GetFlowTemplate(criteria);
            }

            return flowTemplates;
        }

        public IEnumerable<FlowModel> GetDepartmentFlow(GetDepartmentFlowCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<FlowModel> flowDepartment = new List<FlowModel>();
            if (criteria.IsValid())
            {
                flowDepartment = _flowManagementRepository.GetFlowManagement(criteria);
            }

            return flowDepartment;
        }

        public IEnumerable<FlowModel> GetPersonalFlow(GetPersonalFlowCriteria criteria)
        {
            IEnumerable<FlowModel> flowEmployee = new List<FlowModel>();
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                flowEmployee = _flowManagementRepository.GetFlowManagement(criteria);
            }

            return flowEmployee;
        }

        public CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                result = this.Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult DeleteDepartmentFlow(DeleteDepartmentFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }

        public CommandResult DeletePersonalFlow(DeletePersonalFlowCriteria criteria)
        {
            CommandResult result = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }
    }
}