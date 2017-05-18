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
    using Resources;

    public class EvaluationController : BaseController
    {
        private readonly IEmployeeEPAService _evaluationService;

        public EvaluationController()
            : this(null)
        {
        }

        public EvaluationController(IEmployeeEPAService evaluationService)
        {
            _evaluationService = evaluationService ?? new EmployeeEPAService();
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

            epaInfo = _evaluationService.GetEPAInfoById(epaId);
            RemoveSensitiveDataOnEmployeeRole(epaInfo);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
        }

        private void RemoveSensitiveDataOnEmployeeRole(EPAModel epaInfo)
        {
            var roles = SessionService.CurrentUser.Roles.Select(x => x.RoleId).ToList();

            if (!roles.Contains((int)SystemRole.Management))
            {
                foreach (EPAProcessModel epaProcessModel in epaInfo.Process.Managements)
                {
                    epaProcessModel.TotalScore = string.Empty;
                }
            }

            if (!roles.Contains((int)SystemRole.Reviewer))
            {
                foreach (EPAProcessModel epaProcessModel in epaInfo.Process.Reviewers)
                {
                    epaProcessModel.TotalScore = string.Empty;
                }
            }

            if (!roles.Contains((int)SystemRole.Evaluator) || epaInfo.Process.Evaluators.FirstOrDefault()?.StatusId == (int)ProcessStatusType.Open)
            {
                foreach (EPAProcessModel epaProcessModel in epaInfo.Process.Evaluators)
                {
                    epaProcessModel.TotalScore = string.Empty;
                }

                var tests = epaInfo.KPIGroups.SelectMany(x => x.KPIs).SelectMany(x => x.EvaluationByEvaluators);
                foreach (EvaluationModel evaluationModel in tests)
                {
                    evaluationModel.Score = string.Empty;
                    evaluationModel.Rate = 0;
                    evaluationModel.Comment = Lang.NotAvailable;
                }
            }
        }

        [HttpPut]
        public CommandResult ConvertEPA()
        {
            IEnumerable<EPASummaryImportModel> epaInfos = _evaluationService.GetMemberEPASummaryImport();
            var criteria = new UpdateEPASummaryImportCriteria
            {
                EpaInfos = epaInfos
            };

            CommandResult result = _evaluationService.UpdateEPASummaryImport(criteria);
            return result;
        }

        [HttpPut]
        public HttpResponseMessage ResetScore(ResetScoreCriteria criteria)
        {
            HttpResponseMessage response = null;

            if (SessionService.CurrentUser.Roles.Select(x => x.RoleId).Contains((int)SystemRole.SystemAdmin))
            {
                EPAModel epaInfo = _evaluationService.GetEPAInfoById(criteria.EPAId);

                criteria.ModifiedBy = SessionService.EmployeeId;

                CommandResult commandResult;
                if (epaInfo.IsSemiHasMonthly)
                {
                    if (epaInfo.Process.Reviewers.Any()
                        && epaInfo.Process.Reviewers.First().StartDate.Date <= DateTime.Now.Date)
                    {
                        criteria.EPAInfo = epaInfo;

                        commandResult = _evaluationService.ResetScore(criteria);
                        response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                    }
                }
                else
                {
                    criteria.EPAInfo = epaInfo;

                    commandResult = _evaluationService.ResetScore(criteria);
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

            return _evaluationService.CheckEmployeePermission(criteria);
        }
    }
}