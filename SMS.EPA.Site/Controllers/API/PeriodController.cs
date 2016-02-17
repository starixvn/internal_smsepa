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
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web;
    using SMS.EPA.Web.ApiAuthorizeAttribute;
    using SMS.EPA.Web.Services;

    public class PeriodController : BaseController
    {
        private readonly IPeriodService _periodService;
        private readonly IWebCommonService _webCommonService;

        public PeriodController()
            : this(null, null)
        {
        }

        public PeriodController(
            IPeriodService periodService,
            IWebCommonService webCommonService)
        {
            _periodService = periodService ?? new PeriodService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
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
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
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
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
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

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
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
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
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
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult DeletePeriod(int periodId)
        {
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = periodId,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            CommandResult commandResult = _periodService.DeletePeriod(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(
            int periodId,
            int? departmentId = null,
            bool? isLocal = null,
            bool? isExcludedEmployee = null,
            bool? isGeneratedEPA = null,
            int? isEvaluated = null)
        {
            var criteria = new GetPeriodEmployeeSettingCriteria
            {
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                IsExcludedEmployee = isExcludedEmployee,
                IsGeneratedEPA = isGeneratedEPA,
                IsEvaluated = isEvaluated
            };

            IEnumerable<PeriodEmployeeSettingModel> periodEmployeeSettings = _periodService.GetPeriodEmployeeSettingById(criteria);

            var sessionKey = Constants.PeriodEmployeeSettingReportSessionKey + CurrentUser.EmployeeInfo.EmployeeId;
            SessionManager.Set<IEnumerable<PeriodEmployeeSettingModel>>(sessionKey, periodEmployeeSettings);
            return periodEmployeeSettings;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult AddEmployeToPeriod(AssignEmployeePeriodModel model)
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
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdateEmployeeSettingOfPeriod(UpdatedEmployeeSettingPeriodModel model)
        {
            var commandResult = new CommandResult();

            if (ModelState.IsValid)
            {
                var criteria = new UpdateEmployeeSettingOfPeriodCriteria
                {
                    PeriodId = model.PeriodId,
                    EmployeeIds = model.EmployeeIds,
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
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
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

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult GenerateEvaluation(AutoGenerateEvaluationCriteria criteria)
        {
            CommandResult commandResult = _periodService.GenerateEvaluation(criteria);

            if (commandResult.IsSuccessful)
            {
                _periodService.SendEmailNoticeAtBegin(criteria.PeriodId);
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetProcessSettingByPeriodId(int periodId)
        {
            var criteria = new GetProcessSettingPeriodCriteria
            {
                PeriodId = periodId
            };

            IEnumerable<FlowModel> processSettings = _periodService.GetProcessSettingByPeriodId(criteria);
            return processSettings;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdatePeriodProcessSetting(UpdateProcessSettingPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.UpdateProcessSettingPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpGet]
        public bool CheckSelfProcessStarted(int periodId, string employeeIds)
        {
            var criteria = new CheckSelfProcessStartedCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds
            };

            bool hasStarted = _periodService.CheckSelfProcessStarted(criteria);
            return hasStarted;
        }

        public int GetUserPermissionOnPeriod()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.Period);
            return permission;
        }
    }
}