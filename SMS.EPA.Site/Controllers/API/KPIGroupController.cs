// -----------------------------------------------------------------------
// <copyright file="KPIGroupController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.08">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.KPIGroup;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class KPIGroupController : BaseController
    {
        private readonly IKPIGroupService _kpiGroupService;

        public KPIGroupController()
            : this(null)
        {
        }

        public KPIGroupController(IKPIGroupService kpiGroupService)
        {
            _kpiGroupService = kpiGroupService ?? new KPIGroupService();
        }

        [HttpGet]
        public IEnumerable<KPIGroupModel> GetKPIGroup(
            int? targetTypeId = null,
            bool? isActive = null)
        {
            var criteria = new GetKPIGroupCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return _kpiGroupService.GetKPIGroup(criteria);
        }

        [HttpPost]
        public CommandResult InsertKPIGroup(InsertKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _kpiGroupService.InsertKPIGroup(criteria);

            return commandResult;
        }

        [HttpPut]
        public CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _kpiGroupService.UpdateKPIGroup(criteria);

            return commandResult;
        }

        [HttpPut]
        public CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _kpiGroupService.UpdateStatusKPIGroup(criteria);

            return commandResult;
        }

        [HttpDelete]
        public CommandResult DeleteKPIGroup(int kpiGroupId)
        {
            var criteria = new DeleteKPIGroupCriteria
            {
                KPIGroupId = kpiGroupId,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            CommandResult commandResult = _kpiGroupService.DeleteKPIGroup(criteria);

            return commandResult;
        }
    }
}