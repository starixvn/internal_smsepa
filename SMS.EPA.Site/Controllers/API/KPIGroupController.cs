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
    using SMS.EPA.Criteria.KPIGroup;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    /// <summary>
    /// The KPI Group Controller.
    /// </summary>
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
    }
}