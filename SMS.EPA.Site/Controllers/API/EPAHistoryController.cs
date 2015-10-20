// ----------------------------------------------------------------------
// <copyright file="EPAHistoryService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/19/2015">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Model.EPAHistory;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EPAHistoryController : BaseController
    {
        private readonly IEPAHistoryService _epaHistoryService;

        public EPAHistoryController()
            : this(null)
        {
        }

        public EPAHistoryController(IEPAHistoryService epaHistoryService)
        {
            _epaHistoryService = epaHistoryService ?? new EPAHistoryService();
        }

        [HttpGet]
        public IList<EPAHistoryModel> GetEPAHistory(int periodId, int? employeeId = null)
        {
            var criteria = new GetEPAHistoryCriteria
            {
                PeriodId = periodId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                EmployeeId = employeeId
            };

            IList<EPAHistoryModel> epas = _epaHistoryService.GetEPAHistory(criteria);
            return epas;
        }
    }
}
