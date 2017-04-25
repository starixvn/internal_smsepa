namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Criteria.EPAHistory;
    using Model.EPAHistory;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

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
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAHistory, Policy = Policy.Read)]
        public IList<EPAHistoryModel> GetEPAHistory(
            int? periodId = null,
            int? departmentId = null,
            bool? isLocal = null,
            int? employeeId = null)
        {
            var criteria = new GetEPAHistoryCriteria
            {
                PeriodId = periodId,
                EvaluatorId = SessionService.EmployeeId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                EmployeeId = employeeId
            };

            IList<EPAHistoryModel> epas = _epaHistoryService.GetEPAHistory(criteria);
            return epas;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPAHistory, Policy = Policy.Read)]
        public IList<EPAHistoryModel> GetMonthlyEPAHistory(int? periodId = null, int? employeeId = null)
        {
            var criteria = new GetMonthlyEPAHistoryCriteria
            {
                PeriodId = periodId,
                EvaluatorId = SessionService.EmployeeId,
                EmployeeId = employeeId
            };

            IList<EPAHistoryModel> epas = _epaHistoryService.GetMonthlyEPAHistory(criteria);
            return epas;
        }

        [HttpGet]
        public bool HasOnlyEmployeeRole()
        {
            bool onlyEmployeeRole = SessionService.CurrentUser.Roles.All(x => x.RoleId == (int)SystemRole.Employee);
            return onlyEmployeeRole;
        }
    }
}