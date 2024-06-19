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

    public class EvaluatorEPAController : BaseController
    {
        private readonly IEvaluatorEPAService evaluatorService;
        private readonly IBaseService baseService;

        public EvaluatorEPAController() : this(null, null)
        {
        }

        public EvaluatorEPAController(
            IEvaluatorEPAService evaluatorService,
            IBaseService baseService)
        {
            this.evaluatorService = evaluatorService ?? new EvaluatorEPAService();
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodByEvaluator()
        {
            var criteria = new GetPeriodByActorIdCriteria
            {
                ActorId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Evaluator
            };

            return baseService.GetData<EPAPeriodModel>(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByEvaluator(int? periodId = null)
        {
            var criteria = new GetGroupByActorIdCriteria
            {
                ActorUserId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Evaluator,
                PeriodId = periodId
            };

            return baseService.GetData<Group>(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<EPAEmployeeInfo> GetMemberByEvaluator(
            int? periodId = null,
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            var criteria = new GetMemberByActorIdCriteria
            {
                ActorUserId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Evaluator,
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly
            };

            return baseService.GetData<EPAEmployeeInfo>(criteria);
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
                EvaluatorId = CurrentEmployeeId,
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
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator + "," + EPAFunctionKey.Reviewer + "," + EPAFunctionKey.Management, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEvaluator(int epaId, int? evaluatorId = null)
        {
            var epaInfo = new EPAModel();
            var processId = evaluatorId == null ? (byte?)ProcessType.Evaluator : null;
            var sessionServiceEmployeeId = evaluatorId ?? CurrentEmployeeId;
            byte permission = CheckEmployeePermission(epaId, sessionServiceEmployeeId, processId);

            if (permission == (byte)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, epaInfo);
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
                criteria.ProcessId = (byte)ProcessType.Evaluator;
                criteria.EvaluatorId = CurrentEmployeeId;
                criteria.ModifiedBy = CurrentEmployeeId;

                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public byte GetUserPermissionOnEvaluator()
        {
            return GetPermissionOnFunction(EPAFunctionKey.Evaluator);
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