// -----------------------------------------------------------------------
// <copyright file="KPIController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.07">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Criteria.KPI;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class KPIController : ApiController
    {
        private readonly IKPIService _kpiService;

        public KPIController()
            : this(null)
        {
        }

        public KPIController(IKPIService kpiService)
        {
            _kpiService = kpiService ?? new KPIService();
        }

        [HttpGet]
        public List<KPIModel> GetKPI(
            int? kpiGroupId = null,
            bool? isActive = null)
        {
            var criteria = new GetKPICriteria
            {
                KPIGroupId = kpiGroupId,
                IsActive = isActive
            };

            return _kpiService.GetKPI(criteria);
        }
    }
}