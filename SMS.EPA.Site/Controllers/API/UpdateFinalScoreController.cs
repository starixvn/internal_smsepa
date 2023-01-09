namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Services;

    public class UpdateFinalScoreController : BaseController
    {
        private readonly IEmployeeEPAService evaluationService;
        private readonly IBaseService baseService;
        private readonly IEPAProcessService processService;
        private readonly IEmployeeEPAService epaService;

        public UpdateFinalScoreController() : this(null, null, null, null, null)
        {
        }

        public UpdateFinalScoreController(
            IEmployeeEPAService evaluationService,
            ICryptographyService cryptographyService,
            IBaseService baseService,
            IEPAProcessService processService,
            IEmployeeEPAService epaService)
        {
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.baseService = baseService ?? new BaseService();
            this.processService = processService ?? new EPAProcessService();
            this.epaService = epaService ?? new EmployeeEPAService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetPeriodToUpdateFinalScore()
        {
            var criteria = new GetPeriodToUpdateFinalScoreCriteria();
            var periods = baseService.GetData<EPAPeriodModel>(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, periods);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetEmployeeToUpdateFinalScore(
            int periodId,
            string teamIds = null,
            bool? isLocal = null,
            string query = "")
        {
            var criteria = new GetEmployeeToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                TeamIds = teamIds,
                IsLocal = isLocal,
                Query = query
            };
            var employees = baseService.GetData<EPAEmployeeInfo>(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, employees);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAToUpdateFinalScore(
            int periodId,
            string teamIds = null,
            bool? isLocal = null,
            int? employeeId = null)
        {
            var criteria = new GetEPAToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                IsLocal = isLocal,
                TeamIds = teamIds,
                EmployeeId = employeeId
            };

            IList<EPAModel> epaInfos = evaluationService.GetEPAToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Host)]
        public HttpResponseMessage UpdateFinalScore(UpdateFinalScoreParam param)
        {
            CommandResult commandResult = processService.UpdateTotalScoreOfProcess(param.PeriodId, param.EmployeeIds);
            if (commandResult.IsSuccessful)
            {
                commandResult = epaService.GetDataAndUpdateFinalScore(CurrentEmployeeId, param.PeriodId, param.EmployeeIds);
            }

            return Request.CreateResponse(HttpStatusCode.OK, commandResult);
        }
    }
}