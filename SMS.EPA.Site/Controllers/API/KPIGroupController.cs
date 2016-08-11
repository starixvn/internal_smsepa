namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class KPIGroupController : BaseController
    {
        private readonly IKPIGroupService kpiGroupService;

        public KPIGroupController()
            : this(null)
        {
        }

        public KPIGroupController(IKPIGroupService kpiGroupService)
        {
            this.kpiGroupService = kpiGroupService ?? new KPIGroupService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Read)]
        public IEnumerable<KPIGroupModel> GetKPIGroup(bool? isActive = null)
        {
            var criteria = new GetKPIGroupCriteria
            {
                IsActive = isActive
            };

            return kpiGroupService.GetKPIGroup(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Read)]
        public KPIGroupModel GetKPIGroupById(int kpiGroupId)
        {
            var criteria = new GetKPIGroupByIdCriteria
            {
                KPIGroupId = kpiGroupId
            };

            return kpiGroupService.GetKPIGroupById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = kpiGroupService.CreateKPIGroup(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = kpiGroupService.UpdateKPIGroup(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = kpiGroupService.UpdateStatusKPIGroup(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult DeleteKPIGroup(int kpiGroupId)
        {
            var criteria = new DeleteKPIGroupCriteria
            {
                KPIGroupId = kpiGroupId,
                ModifiedBy = this.SessionService.EmployeeId
            };

            CommandResult commandResult = kpiGroupService.DeleteKPIGroup(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnKPIGroup()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.KPIGroup);
            return permission;
        }
    }
}