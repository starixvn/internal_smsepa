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
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class SelfEvaluationController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly ISelfEvaluationService selfEvaluationService;

        public SelfEvaluationController()
            : this(null, null)
        {
        }

        public SelfEvaluationController(
            IEmployeeEPAService evaluationService,
            ISelfEvaluationService selfEvaluationService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.selfEvaluationService = selfEvaluationService ?? new SelfEvaluationService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEmployee()
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = this.SessionService.EmployeeId
            };

            return selfEvaluationService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEmployee(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByEmployeeCriteria()
                    {
                        EPAId = epaId,
                        EmployeeId = this.SessionService.EmployeeId
                    };

                    epaInfo = selfEvaluationService.GetEPAInfoByEmployee(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEmployee(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Self;
                criteria.EvaluatorId = this.SessionService.EmployeeId;
                criteria.ModifiedBy = this.SessionService.EmployeeId;

                commandResult = selfEvaluationService.UpdateEPAScoreByEmployee(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnEmployee()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.Self);
            return permission;
        }

        private int CheckEmployeePermission(int epaId)
        {
            var criteria = new GetEmployeePermissionCriteria()
            {
                EPAId = epaId,
                ActorUserId = this.SessionService.EmployeeId,
                ProcessId = (int)ProcessType.Self
            };

            return evaluationService.CheckEmployeePermission(criteria);
        }
    }
}