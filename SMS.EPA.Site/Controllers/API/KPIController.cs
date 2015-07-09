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
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.KPI;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    /// <summary>
    /// The KPI Controller.
    /// </summary>
    public class KPIController : BaseController
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
        public IEnumerable<KPIModel> GetKPI(
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

        [HttpPost]
        public CommandResult InsertKPI(InsertKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _kpiService.InsertKPI(criteria);

            return commandResult;
        }

        [HttpPut]
        public CommandResult UpdateKPI(UpdateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _kpiService.UpdateKPI(criteria);

            return commandResult;
        }

        [HttpPut]
        public CommandResult UpdateStatusKPI(UpdateStatusKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _kpiService.UpdateStatusKPI(criteria);

            return commandResult;
        }

        [HttpDelete]
        public CommandResult DeleteKPI(int kpiId)
        {
            var criteria = new DeleteKPICriteria
            {
                KPIId = kpiId,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            CommandResult commandResult = _kpiService.DeleteKPI(criteria);

            return commandResult;
        }
    }
}