namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class PeriodController : BaseController
    {
        private readonly IPeriodService periodService;
        private readonly IBaseService baseService;
        private readonly IEPAProcessService processService;
        private readonly IEmployeeEPAService epaService;

        public PeriodController() : this(null, null, null, null)
        {
        }

        public PeriodController(
            IPeriodService periodService,
            IBaseService baseService,
            IEPAProcessService processService,
            IEmployeeEPAService epaService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.baseService = baseService ?? new BaseService();
            this.processService = processService ?? new EPAProcessService();
            this.epaService = epaService ?? new EmployeeEPAService();
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
            return periodService.GetPeriodById(periodId);
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
            return baseService.ExecuteNonQuery(criteria);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
#pragma warning disable S107 // Methods should not have too many parameters
        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(
            int periodId,
            int? departmentId = null,
            bool? isLocal = null,
            bool? isExcludedEmployee = null,
            bool? isGeneratedEPA = null,
            int? isEvaluated = null,
            bool? hasMonthlyEPA = null,
            int? epaStatus = null)
#pragma warning restore S107 // Methods should not have too many parameters
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
            IEnumerable<PeriodEmployeeSettingModel> employeeSettings = periodService.GetPeriodEmployeeSettingById(criteria);
            if (employeeSettings.Any())
            {
                PeriodEmployeeSettingModel employeeSetting = employeeSettings.FirstOrDefault(x => x.TargetTemplate != null);
                int targetTypeId = employeeSetting?.TargetTemplate.TargetTypeId ?? 0;
                switch (targetTypeId)
                {
                    case (byte)TargetType.Probation:
                    case (byte)TargetType.Promotion:
                        employeeSettings = employeeSettings.OrderByDescending(x => x.SelfEvaluators.First().StartDate);
                        break;

                    case (byte)TargetType.Monthly:
                    case (byte)TargetType.SemiAnnual:
                        employeeSettings = employeeSettings.OrderBy(x => x.Employee.Group != null ? x.Employee.Group.GroupCode : string.Empty)
                            .ThenBy(x => x.Employee.NickName);
                        break;

                    default:
                        // do nothing
                        break;
                }
            }

            return employeeSettings;
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

                if (commandResult.IsSuccessful)
                {
                    Task.Factory.StartNew(() =>
                    {
                        processService.UpdateTotalScoreOfProcess(criteria.PeriodId, criteria.EmployeeIds);
                        epaService.GetDataAndUpdateFinalScore(criteria.ModifiedBy, criteria.PeriodId, criteria.EmployeeIds);
                    });
                }
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
            return baseService.GetData<FlowModel>(criteria);
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
            return baseService.GetDataEntity<bool>(criteria);
        }

        public byte GetUserPermissionOnPeriod()
        {
            return SessionService.GetPermission(EPAFunctionKey.Period);
        }
    }
}