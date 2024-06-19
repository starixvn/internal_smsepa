namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.Role;
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

        public EPAHistoryController()
            : this(null)
        {
        }

        public EPAHistoryController(IEPAHistoryService epaHistoryService)
        {
            this.epaHistoryService = epaHistoryService ?? new EPAHistoryService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAHistory, Policy = Policy.Read)]
        public IList<EPAHistoryModel> GetEPAHistory([FromUri] GetEPAHistoryCriteria criteria)
        {
            criteria.EvaluatorId = CurrentEmployeeId;
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
            var roles = UserControlSession.Get<IList<Role>>(Constants.CurrentEmployeeRoleKey);
            return roles != null && roles.All(x => x.RoleId == (byte)SystemRole.Employee);
        }
    }
}