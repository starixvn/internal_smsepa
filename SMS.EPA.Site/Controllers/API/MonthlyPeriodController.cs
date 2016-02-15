// -----------------------------------------------------------------------
// <copyright file="MonthlyPeriodController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.02.15">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.MonthlyPeriod;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web.ApiAuthorizeAttribute;
    using SMS.EPA.Web.Services;

    public class MonthlyPeriodController : BaseController
    {
        private readonly IMonthlyPeriodService _monthlyPeriodService;
        private readonly IWebCommonService _webCommonService;

        public MonthlyPeriodController()
            : this(null, null)
        {
        }

        public MonthlyPeriodController(
            IMonthlyPeriodService monthlyPeriodService,
            IWebCommonService webCommonService)
        {
            _monthlyPeriodService = monthlyPeriodService ?? new MonthlyPeriodService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Read)]
        public IEnumerable<PeriodModel> GetMonthlyPeriod(
            DateTime fromDate,
            DateTime toDate)
        {
            var criteria = new GetMonthlyPeriodCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            IEnumerable<PeriodModel> periods = _monthlyPeriodService.GetMonthlyPeriod(criteria);

            return periods;
        }

        public int GetUserPermissionOnMonthlyPeriod()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.MonthlyPeriod);

            return permission;
        }
    }
}