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
        private readonly IEmployeeEPAService _evaluationService;
        private readonly IReviewerEPAService _reviewerEPAService;

        public ReviewerEPAController()
            : this(null, null)
        {
        }

        public ReviewerEPAController(
            IEmployeeEPAService evaluationService,
            IReviewerEPAService reviewerEPAService)
        {
            _evaluationService = evaluationService ?? new EmployeeEPAService();
            _reviewerEPAService = reviewerEPAService ?? new ReviewerEPAService();
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

            return _evaluationService.GetGroupByEvaluatorOrReviewerOrManagement(criteria);
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

            return _evaluationService.GetMemberByEvaluatorOrReviewerOrManagement(criteria);
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

            return _reviewerEPAService.GetEPAByReviewer(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByReviewer(int epaId, int? reviewerId = null)
        {
            EPAModel epaInfo = new EPAModel();

            var processId = reviewerId == null ? (int?)ProcessType.Reviewer : null;
            reviewerId = reviewerId ?? SessionService.EmployeeId;

            int permission = CheckEmployeePermission(epaId, reviewerId.Value, processId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByReviewerCriteria
                    {
                        EPAId = epaId,
                        ReviewerId = SessionService.EmployeeId
                    };

                    epaInfo = _reviewerEPAService.GetEPAInfoByReviewer(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
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

                commandResult = _reviewerEPAService.AdjustScoreByReviewer(criteria);
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

            return _evaluationService.CheckEmployeePermission(criteria);
        }
    }
}