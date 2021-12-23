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

    public class ReviewerEPAController : BaseController
    {
        private readonly IReviewerEPAService reviewerEPAService;
        private readonly IBaseService baseService;

        public ReviewerEPAController() : this(null, null)
        {
        }

        public ReviewerEPAController(
            IReviewerEPAService reviewerEPAService,
            IBaseService baseService)
        {
            this.reviewerEPAService = reviewerEPAService ?? new ReviewerEPAService();
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodByReviewer()
        {
            var criteria = new GetPeriodByActorIdCriteria
            {
                ActorId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Reviewer
            };

            return baseService.GetData<EPAPeriodModel>(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByReviewer(int? periodId = null)
        {
            var criteria = new GetGroupByActorIdCriteria
            {
                ActorUserId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Reviewer,
                PeriodId = periodId
            };

            return baseService.GetData<Group>(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<EPAEmployeeInfo> GetMemberByReviewer(
            int? periodId = null,
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            var criteria = new GetMemberByActorIdCriteria
            {
                ActorUserId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Reviewer,
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly
            };

            return baseService.GetData<EPAEmployeeInfo>(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByReviewer(
            int? periodId = null,
            int? groupId = null,
            bool? isLocal = null,
            bool isMonthly = false,
            string employeeIds = null,
            bool? statusProcess = null)
        {
            var criteria = new GetEPAByReviewerCriteria
            {
                ReviewerId = CurrentEmployeeId,
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly,
                EmployeeIds = employeeIds,
                StatusProcess = statusProcess
            };

            return reviewerEPAService.GetEPAByReviewer(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByReviewer(int epaId, int? reviewerId = null)
        {
            var epaInfo = new EPAModel();
            var processId = reviewerId == null ? (byte?)ProcessType.Reviewer : null;
            var sessionServiceEmployeeId = reviewerId ?? CurrentEmployeeId;
            byte permission = CheckEmployeePermission(epaId, sessionServiceEmployeeId, processId);

            if (permission == (byte)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);
            }

            var criteria = new GetEPAInfoByReviewerCriteria
            {
                EPAId = epaId,
                ReviewerId = CurrentEmployeeId
            };

            epaInfo = reviewerEPAService.GetEPAInfoByReviewer(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Host)]
        public CommandResult AdjustScoreByReviewer(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            var commandResult = new CommandResult();
            if (criteria != null)
            {
                criteria.ProcessId = (byte)ProcessType.Reviewer;
                criteria.ModifiedBy = CurrentEmployeeId;

                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public byte GetUserPermissionOnReviewer()
        {
            return GetPermissionOnFunction(EPAFunctionKey.Reviewer);
        }

        private byte CheckEmployeePermission(
            int epaId,
            int actorUserId,
            byte? processId = null)
        {
            var criteria = new GetEmployeePermissionCriteria
            {
                EPAId = epaId,
                ActorUserId = actorUserId,
                ProcessId = processId
            };

            return baseService.GetDataEntity<byte>(criteria);
        }
    }
}