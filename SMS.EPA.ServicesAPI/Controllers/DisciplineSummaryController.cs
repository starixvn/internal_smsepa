namespace SMS.EPA.ServicesAPI.Controllers
{
    using Core.Model;
    using Criteria.DisciplineSummary;
    using Criteria.EmployeeDiscipline;
    using Services;
    using System;
    using System.Net;
    using System.Web.Http;
    using EPA.Model;

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

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForDisciplineSummaryCriteria();

            DateTime fromDate = _service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(DisciplineSummary[] disciplines)
        {
            var criteria = new UpdateDisciplineSummaryCriteria
            {
                Disciplines = disciplines
            };

            CommandResult result = _service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}