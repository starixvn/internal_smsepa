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

    public class EvaluatorEPAController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly IEvaluatorEPAService evaluatorService;
        private readonly ICommonService commonService;

        public EvaluatorEPAController() : this(null, null, null)
        {
        }

        public EvaluatorEPAController(
            IEmployeeEPAService evaluationService,
            IEvaluatorEPAService evaluatorService,
            ICommonService commonService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.evaluatorService = evaluatorService ?? new EvaluatorEPAService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodByEvaluator()
        {
            var criteria = new GetPeriodByActorIdCriteria
            {
                ActorId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Evaluator
            };

            return evaluationService.GetPeriodByActorId(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByEvaluator(int? periodId = null)
        {
            var criteria = new GetGroupByActorIdCriteria
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Evaluator,
                PeriodId = periodId
            };

            return evaluationService.GetGroupByActorId(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByEvaluator(
            int? periodId = null,
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            var criteria = new GetMemberByActorIdCriteria
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Evaluator,
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly
            };

            return evaluationService.GetMemberByActorId(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEvaluator(
            int? periodId = null,
            int? groupId = null,
            bool? isLocal = null,
            bool isMonthly = false,
            string employeeIds = null,
            bool? statusProcess = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria
            {
                EvaluatorId = SessionService.EmployeeId,
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly,
                EmployeeIds = employeeIds,
                StatusProcess = statusProcess
            };

            return evaluatorService.GetEPAByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEvaluator(int epaId, int? evaluatorId = null)
        {
            var epaInfo = new EPAModel();
            var processId = evaluatorId == null ? (int?)ProcessType.Evaluator : null;
            var sessionServiceEmployeeId = evaluatorId ?? SessionService.EmployeeId;
            int permission = CheckEmployeePermission(epaId, sessionServiceEmployeeId, processId);

            if (permission == (int)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);
            }

            var criteria = new GetEPAInfoByEvaluatorCriteria
            {
                EPAId = epaId,
                EvaluatorId = sessionServiceEmployeeId
            };

            epaInfo = evaluatorService.GetEPAInfoByEvaluator(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEvaluator(UpdateEPAScoreCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Evaluator;
                criteria.EvaluatorId = SessionService.EmployeeId;
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = commonService.ExecuteNonQuery(criteria);
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

            return evaluationService.CheckEmployeePermission(criteria);
        }
    }
}