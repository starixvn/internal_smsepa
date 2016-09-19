namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.DisciplineSummary;
    using SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class DisciplineSummaryController : ApiController
    {
         private readonly IServiceAPI service;

        public DisciplineSummaryController() : this(null)
        {
        }

        public DisciplineSummaryController(IServiceAPI service)
        {
            this.service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
         {
             var criteria = new GetLastModifiedDateForDisciplineSummaryCriteria();

             DateTime fromDate = service.GetLastModifiedDate(criteria);

             return Ok(fromDate);
         }

         public IHttpActionResult Update(DisciplineSummary[] disciplines)
         {
             var criteria = new UpdateDisciplineSummaryCriteria()
             {
                 Disciplines = disciplines
             };

             CommandResult result = service.Update(criteria);

             return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
         }
    }
}
