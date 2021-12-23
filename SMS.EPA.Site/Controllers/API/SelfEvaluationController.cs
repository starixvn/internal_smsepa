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
    using SMS.EPA.Services;

    public class SelfEvaluationController : BaseController
    {
        private readonly ISelfEvaluationService selfEvaluationService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IBaseService baseService;

        public SelfEvaluationController() : this(null, null, null)
        {
        }

        public SelfEvaluationController(
            ISelfEvaluationService selfEvaluationService,
            IEmployeeEPAService employeeEPAService,
            IBaseService baseService)
        {
            this.selfEvaluationService = selfEvaluationService ?? new SelfEvaluationService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEmployee()
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = CurrentEmployeeId
            };

            return selfEvaluationService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEmployee(int epaId)
        {
            var epaInfo = new EPAModel();
            byte permission = CheckEmployeePermission(epaId);

            if (permission == (byte)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);
            }

            var criteria = new GetEPAInfoByEmployeeCriteria
            {
                EPAId = epaId,
                EmployeeId = CurrentEmployeeId
            };

            epaInfo = selfEvaluationService.GetEPAInfoByEmployee(criteria);

            if (epaInfo.IsProbation)
            {
                epaInfo.ExtendedQuestion = employeeEPAService.GetExtendedQuestionByEPAId(epaInfo.EPAId);
            }

            return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEmployee(UpdateEPAScoreCriteria criteria)
        {
            var commandResult = new CommandResult();
            if (criteria != null)
            {
                criteria.ProcessId = (byte)ProcessType.Self;
                criteria.EvaluatorId = CurrentEmployeeId;
                criteria.ModifiedBy = CurrentEmployeeId;

                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public byte GetUserPermissionOnEmployee()
        {
            return GetPermissionOnFunction(EPAFunctionKey.Self);
        }

        private byte CheckEmployeePermission(int epaId)
        {
            var criteria = new GetEmployeePermissionCriteria
            {
                EPAId = epaId,
                ActorUserId = CurrentEmployeeId,
                ProcessId = (byte)ProcessType.Self
            };

            return baseService.GetDataEntity<byte>(criteria);
        }
    }
}