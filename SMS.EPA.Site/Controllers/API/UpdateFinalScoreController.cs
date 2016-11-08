namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class UpdateFinalScoreController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly ICryptographyService cryptographyService;

        public UpdateFinalScoreController()
            : this(null, null)
        {
        }

        public UpdateFinalScoreController(IEmployeeEPAService evaluationService, ICryptographyService cryptographyService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.cryptographyService = cryptographyService ?? new CryptographyService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetPeriodToUpdateFinalScore()
        {
            var criteria = new GetPeriodToUpdateFinalScoreCriteria();
            IList<EPAPeriodModel> periods = evaluationService.GetPeriodToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, periods);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetEmployeeToUpdateFinalScore(
            int periodId,
            int? groupId = null,
            bool? isLocal = null,
            string query = "")
        {
            var criteria = new GetEmployeeToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                Query = query
            };
            IList<EPAEmployeeModel> employees = evaluationService.GetEmployeeToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, employees);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAToUpdateFinalScore(
            int periodId,
            int? groupId = null,
            bool? isLocal = null,
            int? employeeId = null)
        {
            var criteria = new GetEPAToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                EmployeeId = employeeId
            };
            IList<EPAModel> epaInfos = evaluationService.GetEPAToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Host)]
        public HttpResponseMessage UpdateFinalScore(List<EPAModel> epaInfos)
        {
            IEnumerable<StandardGradeModel> standardGrades = evaluationService.GetStandardGrade();

            foreach (EPAModel epaInfo in epaInfos)
            {
                float? averagePerformanceScore = GetFinalScoreOfEPA(epaInfo),
                    finalScore = averagePerformanceScore.HasValue ? (averagePerformanceScore.Value + epaInfo.DisciplineScore) : (float?)null;

                epaInfo.FinalScore = finalScore.HasValue ? cryptographyService.Encrypt(finalScore.Value.ToString()) : string.Empty;
                string grade = finalScore.HasValue ? evaluationService.ConvertScoreToGrade(finalScore.Value, standardGrades) : string.Empty;
                epaInfo.Grade = !string.IsNullOrEmpty(grade) ? cryptographyService.Encrypt(grade) : string.Empty;
            }

            var criteria = new UpdateFinalScoreEPAEndEvaluationCriteria
            {
                ModifiedBy = this.SessionService.EmployeeId,
                EPAInfos = epaInfos
            };

            CommandResult commandResult = evaluationService.UpdateFinalScoreEPAEndEvaluation(criteria);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, commandResult);

            return response;
        }

        private float? GetFinalScoreOfEPA(EPAModel epaInfo)
        {
            float? finalScore = null;

            if (epaInfo.Process.Managements.Any() && epaInfo.Process.AverageScoreByManagements.HasValue)
            {
                finalScore = epaInfo.Process.AverageScoreByManagements.Value;
            }
            else if (epaInfo.Process.Reviewers.Any() && epaInfo.Process.AverageScoreByReviewers.HasValue)
            {
                finalScore = epaInfo.Process.AverageScoreByReviewers.Value;
            }

            return finalScore;
        }
    }
}