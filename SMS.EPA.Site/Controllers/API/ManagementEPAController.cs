namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Model.Enitites;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class ManagementEPAController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly IManagementEPAService managementEPAService;
        private readonly ICommonService commonService;

        public ManagementEPAController() : this(null, null, null)
        {
        }

        public ManagementEPAController(
            IEmployeeEPAService evaluationService,
            IManagementEPAService managementEPAService,
            ICommonService commonService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.managementEPAService = managementEPAService ?? new ManagementEPAService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByManagement()
        {
            var criteria = new GetGroupByEvaluatorOrReviewerOrManagementCriteria
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Management
            };

            return evaluationService.GetGroupByEvaluatorOrReviewerOrManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByManagement(
            int? groupId = null,
            bool? isLocal = null)
        {
            var criteria = new GetMemberByEvaluatorOrReviewerOrManagementCriteria
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Management,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = false
            };

            return evaluationService.GetMemberByEvaluatorOrReviewerOrManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByManagement(
            int? groupId = null,
            bool? isLocal = null,
            string employeeIds = null,
            bool? statusProcess = null)
        {
            var criteria = new GetEPAByManagementCriteria
            {
                ManagementId = SessionService.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                EmployeeIds = employeeIds,
                StatusProcess = statusProcess
            };

            return managementEPAService.GetEPAByManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByManagement(int epaId, int? managementId = null)
        {
            EPAModel epaInfo = new EPAModel();

            var processId = managementId == null ? (int?)ProcessType.Management : null;
            var sessionServiceEmployeeId = managementId ?? SessionService.EmployeeId;

            int permission = CheckEmployeePermission(epaId, sessionServiceEmployeeId, processId);

            if (permission == (int)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);
            }

            var criteria = new GetEPAInfoByManagementCriteria
            {
                EPAId = epaId,
                ManagementId = SessionService.EmployeeId
            };

            epaInfo = managementEPAService.GetEPAInfoByManagement(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Host)]
        public CommandResult AdjustScoreByManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Management;
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = commonService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnManagement()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.Management);
            return permission;
        }

        private int CheckEmployeePermission(
            int epaId,
            int actorUserId,
            int? processId = null)
        {
            var criteria = new GetEmployeePermissionCriteria
            {
                EPAId = epaId,
                ActorUserId = actorUserId,
                ProcessId = processId
            };

            return evaluationService.CheckEmployeePermission(criteria);
        }
    }
}