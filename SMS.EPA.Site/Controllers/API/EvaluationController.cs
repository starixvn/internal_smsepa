namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EvaluationController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;

        public EvaluationController()
            : this(null)
        {
        }

        public EvaluationController(IEmployeeEPAService evaluationService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
        }

        [HttpGet]
        public HttpResponseMessage GetEPAInfoById(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    epaInfo = evaluationService.GetEPAInfoById(epaId);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        public CommandResult ConvertEPA()
        {
            var getEPASummaryImportCriteria = new GetEPASummaryImportCriteria();
            IEnumerable<EPASummaryImportModel> memberEPAs = evaluationService.GetMemberEPASummaryImport(getEPASummaryImportCriteria);
            var criteria = new UpdateEPASummaryImportCriteria()
            {
                MemberEPAs = memberEPAs
            };

            CommandResult result = evaluationService.UpdateEPASummaryImport(criteria);
            return result;
        }

        [HttpPut]
        public HttpResponseMessage ResetScore(int epaId)
        {
            HttpResponseMessage response = null;

            if (this.SessionService.CurrentUser.Roles.Select(x => x.RoleId).Contains((int)SystemRole.SystemAdmin))
            {
                EPAModel epaInfo = evaluationService.GetEPAInfoById(epaId);

                var resetScoreCriteria = new ResetScoreCriteria()
                {
                    EPAId = epaInfo.EPAId,
                    ModifiedBy = this.SessionService.EmployeeId
                };

                CommandResult commandResult;
                if (epaInfo.IsSemiHasMonthly)
                {
                    if (epaInfo.Process.Reviewers.Any()
                        && epaInfo.Process.Reviewers.First().StartDate.Date <= DateTime.Now.Date)
                    {
                        resetScoreCriteria.EPAInfo = epaInfo;

                        commandResult = evaluationService.ResetScore(resetScoreCriteria);
                        response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                    }
                }
                else
                {
                    resetScoreCriteria.EPAInfo = epaInfo;

                    commandResult = evaluationService.ResetScore(resetScoreCriteria);
                    response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }

            return response;
        }

        private int CheckEmployeePermission(int epaId)
        {
            var criteria = new GetEmployeePermissionCriteria()
            {
                EPAId = epaId,
                ActorUserId = this.SessionService.EmployeeId
            };

            return evaluationService.CheckEmployeePermission(criteria);
        }
    }
}