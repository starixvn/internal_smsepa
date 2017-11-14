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
    using Criteria;
    using Model;
    using Model.Enitites;
    using Services.Implement;
    using Services.Interfaces;

    public class ReviewerEPAController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly IReviewerEPAService reviewerEPAService;
        private readonly ICommonService commonService;

        public ReviewerEPAController() : this(null, null, null)
        {
        }

        public ReviewerEPAController(
            IEmployeeEPAService evaluationService,
            IReviewerEPAService reviewerEPAService,
            ICommonService commonService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.reviewerEPAService = reviewerEPAService ?? new ReviewerEPAService();
            this.commonService = commonService ?? new CommonService();
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

            return evaluationService.GetPeriodByActorId(criteria);
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

            return evaluationService.GetGroupByActorId(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByReviewer(
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

            return evaluationService.GetMemberByActorId(criteria);
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

                commandResult = commonService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnReviewer()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.Reviewer);
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