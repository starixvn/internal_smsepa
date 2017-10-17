namespace SMS.EPA.ServicesAPI.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core.Model;
    using Criteria.DisciplineSummary;
    using Criteria.EmployeeDiscipline;
    using EPA.Model.API;
    using Services;

    [Authorize]
    public class DisciplineSummaryController : ApiController
    {
        private readonly IServiceAPI _service;

        public DisciplineSummaryController() : this(null)
        {
        }

        public DisciplineSummaryController(IServiceAPI service)
        {
            _service = service ?? new ServiceAPI();
        }

        public async Task<IHttpActionResult> GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForDisciplineSummaryCriteria();
            DateTime fromDate = await _service.GetLastModifiedDate(criteria).ConfigureAwait(false);

            return Ok(fromDate);
        }

        public async Task<IHttpActionResult> Update(DisciplineSummary[] disciplines)
        {
            var criteria = new UpdateDisciplineSummaryCriteria
            {
                Disciplines = disciplines
            };
            CommandResult result = await _service.Update(criteria).ConfigureAwait(false);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}