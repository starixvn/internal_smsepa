namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class SelfEvaluationController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly ISelfEvaluationService selfEvaluationService;
        private readonly ICommonService commonService;

        public SelfEvaluationController() : this(null, null, null)
        {
        }

        public SelfEvaluationController(
            IEmployeeEPAService evaluationService,
            ISelfEvaluationService selfEvaluationService,
            ICommonService commonService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.selfEvaluationService = selfEvaluationService ?? new SelfEvaluationService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEmployee()
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = SessionService.EmployeeId
            };

            return selfEvaluationService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEmployee(int epaId)
        {
            var epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId);

            if (permission == (int)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);
            }

            var criteria = new GetEPAInfoByEmployeeCriteria
            {
                EPAId = epaId,
                EmployeeId = SessionService.EmployeeId
            };

            epaInfo = selfEvaluationService.GetEPAInfoByEmployee(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEmployee(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Self;
                criteria.EvaluatorId = SessionService.EmployeeId;
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = commonService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnEmployee()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.Self);
            return permission;
        }

        private int CheckEmployeePermission(int epaId)
        {
            var criteria = new GetEmployeePermissionCriteria
            {
                EPAId = epaId,
                ActorUserId = SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Self
            };

            return evaluationService.CheckEmployeePermission(criteria);
        }
    }
}