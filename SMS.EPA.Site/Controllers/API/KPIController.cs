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
        private readonly IKPIService _kpiService;

        public KPIController()
            : this(null)
        {
        }

        public KPIController(IKPIService kpiService)
        {
            _kpiService = kpiService ?? new KPIService();
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

            return _kpiService.GetKPI(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult CreateKPI(CreateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _kpiService.CreateKPI(criteria);

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

            CommandResult commandResult = _kpiService.UpdateKPI(criteria);

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

            CommandResult commandResult = _kpiService.UpdateStatusKPI(criteria);

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

            CommandResult commandResult = _kpiService.UpdateRequiredEvaluateKPI(criteria);

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

            CommandResult commandResult = _kpiService.DeleteKPI(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnKPI()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.KPI);
            return permission;
        }
    }
}