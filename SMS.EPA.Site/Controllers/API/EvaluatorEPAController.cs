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

    public class EvaluatorEPAController : BaseController
    {
        private readonly IEmployeeEPAService _evaluationService;
        private readonly IEvaluatorEPAService _evaluatorService;

        public EvaluatorEPAController()
            : this(null, null)
        {
        }

        public EvaluatorEPAController(
            IEmployeeEPAService evaluationService,
            IEvaluatorEPAService evaluatorService)
        {
            _evaluationService = evaluationService ?? new EmployeeEPAService();
            _evaluatorService = evaluatorService ?? new EvaluatorEPAService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByEvaluator()
        {
            var criteria = new GetGroupByEvaluatorOrReviewerOrManagementCriteria
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Evaluator
            };

            return _evaluationService.GetGroupByEvaluatorOrReviewerOrManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByEvaluator(
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            var criteria = new GetMemberByEvaluatorOrReviewerOrManagementCriteria
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Evaluator,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly
            };

            return _evaluationService.GetMemberByEvaluatorOrReviewerOrManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEvaluator(
            int? groupId = null,
            bool? isLocal = null,
            bool isMonthly = false,
            string employeeIds = null,
            bool? statusProcess = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria
            {
                EvaluatorId = SessionService.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly,
                EmployeeIds = employeeIds,
                StatusProcess = statusProcess
            };

            return _evaluatorService.GetEPAByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEvaluator(int epaId, int? evaluatorId = null)
        {
            var epaInfo = new EPAModel();
            var processId = evaluatorId == null ? (int?)ProcessType.Evaluator : null;
            evaluatorId = evaluatorId ?? SessionService.EmployeeId;

            int permission = CheckEmployeePermission(epaId, evaluatorId.Value, processId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByEvaluatorCriteria
                    {
                        EPAId = epaId,
                        EvaluatorId = evaluatorId.Value
                    };

                    epaInfo = _evaluatorService.GetEPAInfoByEvaluator(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEvaluator(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Evaluator;
                criteria.EvaluatorId = SessionService.EmployeeId;
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = _evaluatorService.UpdateEPAScoreByEvaluator(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnEvaluator()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.Evaluator);
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