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

    public class KPIGroupController : BaseController
    {
        private readonly IKPIGroupService _kpiGroupService;

        public KPIGroupController()
            : this(null)
        {
        }

        public KPIGroupController(IKPIGroupService kpiGroupService)
        {
            _kpiGroupService = kpiGroupService ?? new KPIGroupService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Read)]
        public IEnumerable<KPIGroupModel> GetKPIGroup(bool? isActive = null)
        {
            var criteria = new GetKPIGroupCriteria
            {
                IsActive = isActive
            };

            return _kpiGroupService.GetKPIGroup(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Read)]
        public KPIGroupModel GetKPIGroupById(int kpiGroupId)
        {
            return _kpiGroupService.GetKPIGroupById(kpiGroupId);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _kpiGroupService.CreateKPIGroup(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _kpiGroupService.UpdateKPIGroup(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _kpiGroupService.UpdateStatusKPIGroup(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult DeleteKPIGroup(int kpiGroupId)
        {
            var criteria = new DeleteKPIGroupCriteria
            {
                KPIGroupId = kpiGroupId,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = _kpiGroupService.DeleteKPIGroup(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnKPIGroup()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.KPIGroup);
            return permission;
        }
    }
}