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
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Flowmanagement;
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
            this._flowManagementRepository = flowManagementRepository ?? new FlowManagementRepository();
        }

        public IEnumerable<EPAFlowModel> GetFlowDepartment(GetDepartmentFlowCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAFlowModel> flowDepartment = new List<EPAFlowModel>();
            if (criteria.IsValid())
            {
                flowDepartment = _flowManagementRepository.GetFlowManagement(criteria);
            }

            return flowDepartment;
        }
    }
}