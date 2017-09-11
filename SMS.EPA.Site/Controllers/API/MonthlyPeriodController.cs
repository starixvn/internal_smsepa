namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    public class MonthlyPeriodController : BaseController
    {
        private readonly IPeriodService periodService;

        public MonthlyPeriodController() : this(null)
        {
        }

        public MonthlyPeriodController(IPeriodService periodService)
        {
            this.periodService = periodService ?? new PeriodService();
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

            IEnumerable<PeriodModel> periods = periodService.GetMonthlyPeriod(criteria);

            return periods;
        }

        public IEnumerable<PeriodModel> GetAllMonthlyPeriod()
        {
            IEnumerable<PeriodModel> periods = periodService.GetAllMonthlyPeriod();

            return periods;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Read)]
        public PeriodModel GetMonthlyPeriodById(int periodId)
        {
            PeriodModel period = periodService.GetPeriodById(periodId);

            return period;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
        public CommandResult CreateMonthlyPeriod(CreatePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.TargetTypeId = (int)TargetType.Monthly;
                criteria.IsActive = true;
                criteria.CreatedBy = SessionService.EmployeeId;
                commandResult = periodService.CreatePeriod(criteria);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
        public CommandResult UpdateBasicInfoMonthlyPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.TargetTypeId = (int)TargetType.Monthly;
                criteria.IsActive = true;
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = periodService.UpdateBasicInformationPeriod(criteria);
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetProcessSettingByPeriodId(int periodId)
        {
            IEnumerable<FlowModel> processSettings = periodService.GetProcessSettingByPeriodId(periodId);
            return processSettings;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
        public CommandResult UpdateProcessSettingMonthlyPeriod(UpdateProcessSettingPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = periodService.UpdateProcessSettingPeriod(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
        public CommandResult DeleteMonthlyPeriod(int periodId)
        {
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = periodId,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = periodService.DeletePeriod(criteria);

            return commandResult;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Read)]
        public IEnumerable<PeriodEmployeeSettingModel> GetEmployeeSettingMonthlyPeriodById(
            int periodId,
            int? departmentId = null,
            bool? isLocal = null,
            bool? isExcludedEmployee = null,
            bool? isGeneratedEPA = null,
            int? isEvaluated = null,
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
                EPAStatus = epaStatus
            };

            IEnumerable<PeriodEmployeeSettingModel> employeeSettings = periodService.GetPeriodEmployeeSettingById(criteria);
            employeeSettings = employeeSettings.OrderBy(x => x.Employee.Group.GroupCode).ThenBy(x => x.Employee.NickName);

            return employeeSettings;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
        public CommandResult AddEmployeeToMonthlyPeriod(AddEmployeeToPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = periodService.AddEmployeeToPeriod(criteria);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
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

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
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

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
        public CommandResult UpdateIncludeStatusEmployeePeriod(UpdateIncludeStatusEmployeePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = periodService.UpdateIncludeStatusEmployeePeriod(criteria);
            }

            return commandResult;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyPeriod, Policy = Policy.Host)]
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

        public int GetUserPermissionOnMonthlyPeriod()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.MonthlyPeriod);

            return permission;
        }
    }
}