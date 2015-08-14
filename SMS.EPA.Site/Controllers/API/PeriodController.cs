// ----------------------------------------------------------------------
// <copyright file="PeriodController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class PeriodController : BaseController
    {
        private readonly IPeriodService _periodService;

        public PeriodController()
            : this(null)
        {
        }

        public PeriodController(IPeriodService periodService)
        {
            _periodService = periodService ?? new PeriodService();
        }

        public IEnumerable<PeriodModel> GetPeriod(int? targetTypeId = null, bool? isActive = true)
        {
            var criteria = new GetPeriodCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            IEnumerable<PeriodModel> periods = _periodService.GetPeriod(criteria);
            return periods;
        }

        [HttpPost]
        public CommandResult CreatePeriod(CreatePeriodCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _periodService.CreatePeriod(criteria);
            return commandResult;
        }

        [HttpPut]
        public CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _periodService.UpdateBasicInformationPeriod(criteria);
            return commandResult;
        }

        public PeriodModel GetPeriodById(int periodId)
        {
            var criteria = new GetPeriodByIdCriteria
            {
                PeriodId = periodId
            };

            PeriodModel period = _periodService.GetPeriodById(criteria);
            return period;
        }

        [HttpPut]
        public CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _periodService.UpdateStatusPeriod(criteria);
            return commandResult;
        }

        [HttpDelete]
        public CommandResult DeletePeriod(int periodId)
        {
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = periodId
            };

            CommandResult commandResult = _periodService.DeletePeriod(criteria);
            return commandResult;
        }

        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(int periodId)
        {
            var criteria = new GetPeriodEmployeeSettingCriteria
            {
                PeriodId = periodId
            };

            IEnumerable<PeriodEmployeeSettingModel> periodEmployeeSettings = _periodService.GetPeriodEmployeeSettingById(criteria);
            return periodEmployeeSettings;
        }
    }
}
