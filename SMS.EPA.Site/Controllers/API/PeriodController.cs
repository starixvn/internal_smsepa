namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class PeriodController : BaseController
    {
        private readonly IPeriodService periodService;
        private readonly IBaseService baseService;

        public PeriodController() : this(null, null)
        {
        }

        public PeriodController(IPeriodService periodService, IBaseService baseService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.baseService = baseService ?? new BaseService();
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
            IEnumerable<PeriodModel> probationPeriods = periods.Where(x => x.TargetType.TargetTypeId == (int)TargetType.Probation);
            IEnumerable<PeriodModel> promotionPeriods = periods.Where(x => x.TargetType.TargetTypeId == (int)TargetType.Promotion)
                .OrderByDescending(x => x.StartDate);
            IEnumerable<PeriodModel> semiPeriods = periods.Where(x => x.TargetType.TargetTypeId == (int)TargetType.SemiAnnual)
                .OrderByDescending(x => x.StartDate);

            IEnumerable<PeriodModel> periodList = new List<PeriodModel>();

            if (probationPeriods.Any())
            {
                periodList = periodList.Concat(probationPeriods);
            }

            if (promotionPeriods.Any())
            {
                periodList = periodList.Concat(promotionPeriods);
            }

            if (semiPeriods.Any())
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
                criteria.CreatedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(criteria);
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
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(criteria);
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
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(criteria);
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
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
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
                int targetTypeId = periodEmployeeSetting?.TargetTemplate.TargetTypeId ?? 0;

                switch (targetTypeId)
                {
                    case (int)TargetType.Probation:
                    case (int)TargetType.Promotion:
                        periodEmployeeSettings = periodEmployeeSettings.OrderByDescending(x => x.SelfEvaluators.First().StartDate);
                        break;

                    case (int)TargetType.Monthly:
                    case (int)TargetType.SemiAnnual:
                        periodEmployeeSettings = periodEmployeeSettings.OrderBy(x => x.Employee.Group.GroupCode).ThenBy(x => x.Employee.NickName);
                        break;

                    default:
                        return periodEmployeeSettings;
                }
            }

            return periodEmployeeSettings;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult AddEmployeToPeriod(AddEmployeeToPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = periodService.UpdateEmployeeSettingOfPeriod(criteria);
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
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult GenerateEvaluation(AutoGenerateEvaluationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                var extendedQuetionEnabled = ConfigurationManagerService.GetAppSetting("ProbationExtendedQuestionEnabled");
                criteria.ExtendedQuestionEnabled = extendedQuetionEnabled.Equals("true", StringComparison.OrdinalIgnoreCase);

                commandResult = baseService.ExecuteNonQuery(criteria);

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
            IEnumerable<FlowModel> processSettings = baseService.GetData<FlowModel>(criteria);
            return processSettings;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdatePeriodProcessSetting(UpdateProcessSettingPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(criteria);
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

            bool hasStarted = baseService.GetDataEntity<bool>(criteria);
            return hasStarted;
        }

        public int GetUserPermissionOnPeriod()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.Period);
            return permission;
        }
    }
}