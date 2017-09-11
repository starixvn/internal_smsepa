namespace SMS.EPA.Site.Controllers.API
{
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

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

            if (permission == (int)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);
            }

            epaInfo = evaluationService.GetEPAInfoById(epaId);
            evaluationService.RemoveSensitiveDataOnEmployeeRole(epaInfo, SessionService);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
        }

        [HttpPut]
        public CommandResult ConvertEPA()
        {
            IEnumerable<EPASummaryImportModel> epaInfos = evaluationService.GetMemberEPASummaryImport();
            var criteria = new UpdateEPASummaryImportCriteria
            {
                EpaInfos = epaInfos
            };

            CommandResult result = evaluationService.UpdateEPASummaryImport(criteria);
            return result;
        }

        [HttpPut]
        public HttpResponseMessage ResetScore(ResetScoreCriteria criteria)
        {
            HttpResponseMessage response = null;

            if (SessionService.CurrentUser.Roles.Select(x => x.RoleId).Contains((int)SystemRole.SystemAdmin))
            {
                EPAModel epaInfo = evaluationService.GetEPAInfoById(criteria.EPAId);

                criteria.ModifiedBy = SessionService.EmployeeId;

                CommandResult commandResult;
                if (epaInfo.IsSemiHasMonthly)
                {
                    if (epaInfo.Process.Reviewers.Any()
                        && epaInfo.Process.Reviewers.First().StartDate.Date <= DateTime.Now.Date)
                    {
                        criteria.EPAInfo = epaInfo;

                        commandResult = evaluationService.ResetScore(criteria);
                        response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                    }
                }
                else
                {
                    criteria.EPAInfo = epaInfo;

                    commandResult = evaluationService.ResetScore(criteria);
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
            var criteria = new GetEmployeePermissionCriteria
            {
                EPAId = epaId,
                ActorUserId = SessionService.EmployeeId
            };

            return evaluationService.CheckEmployeePermission(criteria);
        }
    }
}