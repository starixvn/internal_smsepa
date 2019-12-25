namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

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
                EmployeeId = SessionService.EmployeeId
            };

            return selfEvaluationService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEmployee(int epaId)
        {
            var extendedQuetionEnabled = ConfigurationManagerService.GetAppSetting("ProbationExtendedQuestionEnabled");
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

            if (extendedQuetionEnabled.Equals("true", StringComparison.OrdinalIgnoreCase)){
                epaInfo.ExtendedQuestion = employeeEPAService.GetExtendedQuestionByEPAId(epaInfo.EPAId);
            }

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

                commandResult = baseService.ExecuteNonQuery(criteria);
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

            return baseService.GetDataEntity<int>(criteria);
        }
    }
}