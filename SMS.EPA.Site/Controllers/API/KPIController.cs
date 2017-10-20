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
    using System.Collections.Generic;
    using System.Web.Http;

    public class KPIController : BaseController
    {
        private readonly IKPIService kpiService;
        private readonly ICommonService commonService;

        public KPIController()
            : this(null, null)
        {
        }

        public KPIController(IKPIService kpiService, ICommonService commonService)
        {
            this.kpiService = kpiService ?? new KPIService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Read)]
        public IEnumerable<KPIModel> GetKPI(
            int? kpiGroupId = null,
            bool? isActive = null,
            bool? isRequiredEvaluate = null)
        {
            var criteria = new GetKPICriteria
            {
                KPIGroupId = kpiGroupId,
                IsActive = isActive,
                IsRequiredEvaluate = isRequiredEvaluate
            };

            return kpiService.GetKPI(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult CreateKPI(CreateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult UpdateKPI(UpdateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult UpdateStatusKPI(UpdateStatusKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult UpdateRequiredEvaluateKPI(UpdateRequiredEvaluateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult DeleteKPI(int kpiId)
        {
            var criteria = new DeleteKPICriteria
            {
                KPIId = kpiId,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnKPI()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.KPI);
            return permission;
        }
    }
}