namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ReviewerEPAController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly IReviewerEPAService reviewerEPAService;

        public ReviewerEPAController()
            : this(null, null)
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
            var criteria = new GetGroupByEvaluatorOrReviewerOrManagementCriteria()
            {
                ActorUserId = this.SessionService.EmployeeId,
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
            var criteria = new GetMemberByEvaluatorOrReviewerOrManagementCriteria()
            {
                ActorUserId = this.SessionService.EmployeeId,
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
            var criteria = new GetEPAByReviewerCriteria()
            {
                ReviewerId = this.SessionService.EmployeeId,
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
        public HttpResponseMessage GetEPAInfoByReviewer(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByReviewerCriteria()
                    {
                        EPAId = epaId,
                        ReviewerId = this.SessionService.EmployeeId
                    };

                    epaInfo = reviewerEPAService.GetEPAInfoByReviewer(criteria);

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
                criteria.ModifiedBy = this.SessionService.EmployeeId;

                commandResult = reviewerEPAService.AdjustScoreByReviewer(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnReviewer()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.Reviewer);
            return permission;
        }

        private int CheckEmployeePermission(int epaId)
        {
            var criteria = new GetEmployeePermissionCriteria()
            {
                EPAId = epaId,
                ActorUserId = this.SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Reviewer
            };

            return evaluationService.CheckEmployeePermission(criteria);
        }
    }
}