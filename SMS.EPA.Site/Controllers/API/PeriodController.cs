namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class PeriodController : BaseController
    {
        private readonly IPeriodService periodService;

        public PeriodController()
            : this(null)
        {
        }

        public PeriodController(IPeriodService periodService)
        {
            this.periodService = periodService ?? new PeriodService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
        public IEnumerable<PeriodModel> GetPeriod(int? targetTypeId = null, bool? isActive = true)
        {
            var criteria = new GetPeriodCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            IEnumerable<PeriodModel> periods = periodService.GetPeriod(criteria);
            IEnumerable<PeriodModel> probationPeriods = periods.Where(x => x.TargetType.TargetTypeId == (int)TargetTypes.Probation);
            IEnumerable<PeriodModel> promotionPeriods = periods.Where(
                x => x.TargetType.TargetTypeId == (int)TargetTypes.Promotion)
                .OrderByDescending(x => x.StartDate);
            IEnumerable<PeriodModel> semiPeriods = periods.Where(
                x => x.TargetType.TargetTypeId == (int)TargetTypes.SemiAnnual)
                .OrderByDescending(x => x.StartDate);

            IEnumerable<PeriodModel> periodList = new List<PeriodModel>();

            if (probationPeriods != null && probationPeriods.Any())
            {
                periodList = periodList.Concat(probationPeriods);
            }

            if (promotionPeriods != null && promotionPeriods.Any())
            {
                periodList = periodList.Concat(promotionPeriods);
            }

            if (semiPeriods != null && semiPeriods.Any())
            {
                periodList = periodList.Concat(semiPeriods);
            }

            return periodList;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult CreatePeriod(CreatePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.CreatedBy = this.SessionService.EmployeeId;
                commandResult = periodService.CreatePeriod(criteria);
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
                criteria.ModifiedBy = this.SessionService.EmployeeId;
                commandResult = periodService.UpdateBasicInformationPeriod(criteria);
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
            PeriodModel period = periodService.GetPeriodById(periodId);

            return period;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
                commandResult = periodService.UpdateStatusPeriod(criteria);
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
                ModifiedBy = this.SessionService.EmployeeId
            };

            CommandResult commandResult = periodService.DeletePeriod(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(
            int periodId,
            int? departmentId = null,
            bool? isLocal = null,
            bool? isExcludedEmployee = null,
            bool? isGeneratedEPA = null,
            int? isEvaluated = null,
            bool? hasMonthlyEPA = null,
            int? epaStatus = null)
        {
            var criteria = new GetPeriodEmployeeSettingCriteria
            {
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                IsExcludedEmployee = isExcludedEmployee,
                IsGeneratedEPA = isGeneratedEPA,
                IsEvaluated = isEvaluated,
                HasMonthlyEPA = hasMonthlyEPA,
                EPAStatus = epaStatus
            };

            IEnumerable<PeriodEmployeeSettingModel> periodEmployeeSettings = periodService.GetPeriodEmployeeSettingById(criteria);

            if (periodEmployeeSettings.Any())
            {
                PeriodEmployeeSettingModel periodEmployeeSetting = periodEmployeeSettings.FirstOrDefault(x => x.TargetTemplate != null);
                int targetTypeId = periodEmployeeSetting != null ? periodEmployeeSetting.TargetTemplate.TargetTypeId : 0;

                switch (targetTypeId)
                {
                    case (int)TargetTypes.Probation:
                    case (int)TargetTypes.Promotion:
                        periodEmployeeSettings = periodEmployeeSettings.OrderByDescending(x => x.SelfEvaluators.First().StartDate);
                        break;

                    case (int)TargetTypes.Monthly:
                    case (int)TargetTypes.SemiAnnual:
                        periodEmployeeSettings = periodEmployeeSettings.OrderBy(x => x.Group.GroupCode).ThenBy(x => x.Employee.NickName);
                        break;
                }
            }

            return periodEmployeeSettings;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult AddEmployeToPeriod(AssignEmployeePeriodModel model)
        {
            var commandResult = new CommandResult();

            if (ModelState.IsValid && model != null)
            {
                var criteria = new AddEmployeeToPeriodCriteria
                {
                    PeriodId = model.PeriodId,
                    EmployeeId = model.EmployeeId,
                    Departments = model.ConvertToXml(),
                    ModifiedBy = this.SessionService.EmployeeId
                };

                commandResult = periodService.AddEmployeeToPeriod(criteria);
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

            if (ModelState.IsValid && model != null)
            {
                var criteria = new UpdateEmployeeSettingOfPeriodCriteria
                {
                    PeriodId = model.PeriodId,
                    EmployeeIds = model.EmployeeIds,
                    TargetTemplateId = model.TargetTemplateId,
                    EmployeeAssignments = model.ConvertToXml(),
                    ModifiedBy = this.SessionService.EmployeeId
                };

                commandResult = periodService.UpdateEmployeeSettingOfPeriod(criteria);
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
                criteria.ModifiedBy = this.SessionService.EmployeeId;
                commandResult = periodService.UpdateIncludeStatusEmployeePeriod(criteria);
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
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                commandResult = periodService.GenerateEvaluation(criteria);

                if (commandResult.IsSuccessful)
                {
                    periodService.SendEmailNoticeAtBegin(criteria.PeriodId);
                }
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

            IEnumerable<FlowModel> processSettings = periodService.GetProcessSettingByPeriodId(criteria);
            return processSettings;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdatePeriodProcessSetting(UpdateProcessSettingPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
                commandResult = periodService.UpdateProcessSettingPeriod(criteria);
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

            bool hasStarted = periodService.CheckSelfProcessStarted(criteria);
            return hasStarted;
        }

        public int GetUserPermissionOnPeriod()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.Period);
            return permission;
        }
    }
}