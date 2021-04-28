namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using Model.Enitites;
    using Services.Implement;
    using Services.Interfaces;

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
                ActorId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Reviewer
            };

            return baseService.GetData<EPAPeriodModel>(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByReviewer(int? periodId = null)
        {
            var criteria = new GetGroupByActorIdCriteria
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Reviewer,
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
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Reviewer,
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
                ReviewerId = SessionService.EmployeeId,
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
            var processId = reviewerId == null ? (int?)ProcessType.Reviewer : null;
            var sessionServiceEmployeeId = reviewerId ?? SessionService.EmployeeId;
            int permission = CheckEmployeePermission(epaId, sessionServiceEmployeeId, processId);

            if (permission == (int)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);
            }

            var criteria = new GetEPAInfoByReviewerCriteria
            {
                EPAId = epaId,
                ReviewerId = SessionService.EmployeeId
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
                criteria.ProcessId = (int)ProcessType.Reviewer;
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public byte GetUserPermissionOnReviewer()
        {
            return SessionService.GetPermission(EPAFunctionKey.Reviewer);
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