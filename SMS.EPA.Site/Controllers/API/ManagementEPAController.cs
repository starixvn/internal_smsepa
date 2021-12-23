namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ManagementEPAController : BaseController
    {
        private readonly IManagementEPAService managementEPAService;
        private readonly IBaseService baseService;

        public ManagementEPAController() : this(null, null)
        {
        }

        public ManagementEPAController(
            IManagementEPAService managementEPAService,
            IBaseService baseService)
        {
            this.managementEPAService = managementEPAService ?? new ManagementEPAService();
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodByManagement()
        {
            var criteria = new GetPeriodByActorIdCriteria
            {
                ActorId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Management
            };

            return baseService.GetData<EPAPeriodModel>(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByManagement(int? periodId = null)
        {
            var criteria = new GetGroupByActorIdCriteria
            {
                ActorUserId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Management,
                PeriodId = periodId
            };

            return baseService.GetData<Group>(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<EPAEmployeeInfo> GetMemberByManagement(
            int? periodId = null,
            int? groupId = null,
            bool? isLocal = null)
        {
            var criteria = new GetMemberByActorIdCriteria
            {
                ActorUserId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Management,
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = false
            };

            return baseService.GetData<EPAEmployeeInfo>(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByManagement(
            int? periodId = null,
            int? groupId = null,
            bool? isLocal = null,
            string employeeIds = null,
            bool? statusProcess = null)
        {
            var criteria = new GetEPAByManagementCriteria
            {
                ManagementId = CurrentEmployeeId,
                PeriodId = periodId,
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

            var processId = managementId == null ? (byte?)ProcessType.Management : null;
            var sessionServiceEmployeeId = managementId ?? CurrentEmployeeId;

            int permission = CheckEmployeePermission(epaId, sessionServiceEmployeeId, processId);

            if (permission == (int)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);
            }

            var criteria = new GetEPAInfoByManagementCriteria
            {
                EPAId = epaId,
                ManagementId = CurrentEmployeeId
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
                criteria.ProcessId = (byte)ProcessType.Management;
                criteria.ModifiedBy = CurrentEmployeeId;

                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public byte GetUserPermissionOnManagement()
        {
            return GetPermissionOnFunction(EPAFunctionKey.Management);
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

            return baseService.GetDataEntity<int>(criteria);
        }
    }
}