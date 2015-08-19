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
    using SMS.EPA.Resources;
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
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.CreatePeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpPut]
        public CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.UpdateBasicInformationPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

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
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.UpdateStatusPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

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

        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(
            int periodId,
            int? departmentId = null,
            bool? isExcludedEmployee = null)
        {
            var criteria = new GetPeriodEmployeeSettingCriteria
            {
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsExcludedEmployee = isExcludedEmployee
            };

            IEnumerable<PeriodEmployeeSettingModel> periodEmployeeSettings = _periodService.GetPeriodEmployeeSettingById(criteria);
            return periodEmployeeSettings;
        }

        [HttpPost]
        public CommandResult AddEmployeToPeriod(RawAssignEmployeePeriodModel model)
        {
            var commandResult = new CommandResult();

            if (ModelState.IsValid)
            {
                var criteria = new AddEmployeeToPeriodCriteria
                {
                    PeriodId = model.PeriodId,
                    EmployeeId = model.EmployeeId,
                    Departments = model.ConvertToXml(),
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
                };

                commandResult = _periodService.AddEmployeeToPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpPut]
        public CommandResult UpdateEmployeeSettingOfPeriod(RawUpdatedEmployeeSettingPeriodModel model)
        {
            var commandResult = new CommandResult();

            if (ModelState.IsValid)
            {
                var criteria = new UpdateEmployeeSettingOfPeriodCriteria
                {
                    PeriodId = model.PeriodId,
                    EmployeeId = model.EmployeeId,
                    TargetTemplateId = model.TargetTemplateId,
                    EmployeeAssignments = model.ConvertToXml(),
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
                };

                commandResult = _periodService.UpdateEmployeeSettingOfPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpPut]
        public CommandResult UpdateIncludeStatusEmployeePeriod(UpdateIncludeStatusEmployeePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.UpdateIncludeStatusEmployeePeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }
    }
}
