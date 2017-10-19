namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
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

    public class UpdateFinalScoreController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly ICryptographyService cryptographyService;
        private readonly ICommonService commonService;

        public UpdateFinalScoreController() : this(null, null, null)
        {
        }

        public UpdateFinalScoreController(
            IEmployeeEPAService evaluationService,
            ICryptographyService cryptographyService,
            ICommonService commonService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.cryptographyService = cryptographyService ?? new CryptographyService();
            this.commonService = commonService ?? new CommonService();
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
                float? performanceScore = GetPerformanceScoreOfEPA(epaInfo);
                float? finalScore = performanceScore.HasValue ? (performanceScore.Value + epaInfo.DisciplineScore) : (float?)null;
                finalScore = finalScore.HasValue ? (finalScore.Value > Constants.MaxFinalScore ? Constants.MaxFinalScore : finalScore.Value) : finalScore;

                epaInfo.FinalScore = finalScore.HasValue
                    ? cryptographyService.Encrypt(finalScore.Value.ToString(CultureInfo.InvariantCulture))
                    : string.Empty;
                string grade = performanceScore.HasValue
                    ? evaluationService.ConvertScoreToGrade(performanceScore, epaInfo.DisciplineScore, epaInfo.HasDisciplineScore, standardGrades)
                    : string.Empty;
                epaInfo.Grade = !string.IsNullOrEmpty(grade) ? cryptographyService.Encrypt(grade) : string.Empty;
            }

            var criteria = new UpdateFinalScoreEPAEndEvaluationCriteria
            {
                ModifiedBy = SessionService.EmployeeId,
                EPAInfos = epaInfos
            };

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, commandResult);

            return response;
        }

        private static float? GetPerformanceScoreOfEPA(EPAModel epaInfo)
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