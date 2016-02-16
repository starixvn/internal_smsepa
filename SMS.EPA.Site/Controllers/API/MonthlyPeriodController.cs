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
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.MonthlyPeriod;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Resources;
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

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Read)]
        public PeriodModel GetMonthlyPeriodById(int periodId)
        {
            var criteria = new GetMonthlyPeriodByIdCriteria
            {
                PeriodId = periodId
            };

            PeriodModel period = _monthlyPeriodService.GetMonthlyPeriodById(criteria);
            return period;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
        public CommandResult CreateMonthlyPeriod(CreateMonthlyPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _monthlyPeriodService.CreateMonthlyPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
        public CommandResult UpdateBasicInfoMonthlyPeriod(UpdateBasicInfoMonthlyPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _monthlyPeriodService.UpdateBasicInfoMonthlyPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetProcessSettingByPeriodId(int periodId)
        {
            var criteria = new GetProcessSettingMonthlyPeriodCriteria
            {
                PeriodId = periodId
            };

            IEnumerable<FlowModel> processSettings = _monthlyPeriodService.GetProcessSettingByPeriodId(criteria);

            return processSettings;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
        public CommandResult UpdateProcessSettingMonthlyPeriod(UpdateProcessSettingMonthlyPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;

                commandResult = _monthlyPeriodService.UpdateProcessSettingMonthlyPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        public int GetUserPermissionOnMonthlyPeriod()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.MonthlyPeriod);

            return permission;
        }
    }
}