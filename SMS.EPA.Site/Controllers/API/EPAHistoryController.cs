namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Services;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Model.EPAHistory;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EPAHistoryController : BaseController
    {
        private readonly IEPAHistoryService epaHistoryService;
        private readonly IRoleService roleService;

        public EPAHistoryController()
            : this(null, null)
        {
        }

        public EPAHistoryController(
            IEPAHistoryService epaHistoryService,
            IRoleService roleService)
        {
            this.epaHistoryService = epaHistoryService ?? new EPAHistoryService();
            this.roleService = roleService ?? new RoleService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAHistory, Policy = Policy.Read)]
        public IList<EPAHistoryModel> GetEPAHistory(
            int? periodId = null,
            string teamIds = null,
            bool? isLocal = null,
            int? employeeId = null)
        {
            var criteria = new GetEPAHistoryCriteria
            {
                PeriodId = periodId,
                EvaluatorId = CurrentEmployeeId,
                TeamIds = teamIds,
                IsLocal = isLocal,
                EmployeeId = employeeId
            };

            IList<EPAHistoryModel> epas = epaHistoryService.GetEPAHistory(criteria);
            return epas;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPAHistory, Policy = Policy.Read)]
        public IList<EPAHistoryModel> GetMonthlyEPAHistory(int? periodId = null, int? employeeId = null)
        {
            var criteria = new GetMonthlyEPAHistoryCriteria
            {
                PeriodId = periodId,
                EvaluatorId = CurrentEmployeeId,
                EmployeeId = employeeId
            };

            IList<EPAHistoryModel> epas = epaHistoryService.GetMonthlyEPAHistory(criteria);
            return epas;
        }

        [HttpGet]
        public bool HasOnlyEmployeeRole()
        {
            return roleService.GetByUserId(UserControlSession.UserId)
                .All(x => x.RoleId == (byte)SystemRole.Employee);
        }
    }
}