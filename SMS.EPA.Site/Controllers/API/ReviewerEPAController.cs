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

    public class ReviewerEPAController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly IReviewerEPAService reviewerEPAService;

        public ReviewerEPAController() : this(null, null)
        {
        }

        public ReviewerEPAController(
            IEmployeeEPAService evaluationService,
            IReviewerEPAService reviewerEPAService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.reviewerEPAService = reviewerEPAService ?? new ReviewerEPAService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByReviewer()
        {
            var criteria = new GetGroupByEvaluatorOrReviewerOrManagementCriteria
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Reviewer
            };

            return evaluationService.GetGroupByEvaluatorOrReviewerOrManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByReviewer(
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            var criteria = new GetMemberByEvaluatorOrReviewerOrManagementCriteria
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Reviewer,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly
            };

            return evaluationService.GetMemberByEvaluatorOrReviewerOrManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByReviewer(
            int? groupId = null,
            bool? isLocal = null,
            bool isMonthly = false,
            string employeeIds = null,
            bool? statusProcess = null)
        {
            var criteria = new GetEPAByReviewerCriteria
            {
                ReviewerId = SessionService.EmployeeId,
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

                commandResult = reviewerEPAService.AdjustScoreByReviewer(criteria);
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