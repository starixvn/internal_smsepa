namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class UpdateFinalScoreController : BaseController
    {
        private readonly IEmployeeEPAService _evaluationService;
        private readonly ICryptographyService _cryptographyService;

        public UpdateFinalScoreController()
            : this(null, null)
        {
        }

        public UpdateFinalScoreController(IEmployeeEPAService evaluationService,
            ICryptographyService cryptographyService)
        {
            _evaluationService = evaluationService ?? new EmployeeEPAService();
            _cryptographyService = cryptographyService ?? new CryptographyService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetPeriodToUpdateFinalScore()
        {
            var criteria = new GetPeriodToUpdateFinalScoreCriteria();
            IList<EPAPeriodModel> periods = _evaluationService.GetPeriodToUpdateFinalScore(criteria);

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

            IList<EPAEmployeeModel> employees = _evaluationService.GetEmployeeToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, employees);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAToUpdateFinalScore(
            int periodId,
            int? departmentId = null,
            bool? isLocal = null,
            int? employeeId = null)
        {
            var criteria = new GetEPAToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                IsLocal = isLocal,
                DepartmentId = departmentId,
                EmployeeId = employeeId
            };

            IList<EPAModel> epaInfos = _evaluationService.GetEPAToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Host)]
        public HttpResponseMessage UpdateFinalScore(List<EPAModel> epaInfos)
        {
            IEnumerable<StandardGradeModel> standardGrades = _evaluationService.GetStandardGrade();

            foreach (EPAModel epaInfo in epaInfos)
            {
                float? averagePerformanceScore = GetFinalScoreOfEPA(epaInfo);
                float? finalScore = averagePerformanceScore.HasValue
                        ? (averagePerformanceScore.Value + epaInfo.DisciplineScore)
                        : (float?)null;

                epaInfo.FinalScore = finalScore.HasValue
                    ? _cryptographyService.Encrypt(finalScore.Value.ToString(CultureInfo.InvariantCulture))
                    : string.Empty;
                string grade = finalScore.HasValue
                    ? _evaluationService.ConvertScoreToGrade(finalScore.Value, standardGrades)
                    : string.Empty;
                epaInfo.Grade = !string.IsNullOrEmpty(grade) ? _cryptographyService.Encrypt(grade) : string.Empty;
            }

            var criteria = new UpdateFinalScoreEPAEndEvaluationCriteria
            {
                ModifiedBy = SessionService.EmployeeId,
                EPAInfos = epaInfos
            };

            CommandResult commandResult = _evaluationService.UpdateFinalScoreEPAEndEvaluation(criteria);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, commandResult);

            return response;
        }

        private static float? GetFinalScoreOfEPA(EPAModel epaInfo)
        {
            float? finalScore = null;

            if (epaInfo.Process.Managements.Any() && epaInfo.Process.AverageScoreByManagements.HasValue)
            {
                finalScore = epaInfo.Process.AverageScoreByManagements.Value;
            }
            else
            {
                if (epaInfo.Process.Reviewers.Any() && epaInfo.Process.AverageScoreByReviewers.HasValue)
                {
                    finalScore = epaInfo.Process.AverageScoreByReviewers.Value;
                }
            }

            return finalScore;
        }
    }
}