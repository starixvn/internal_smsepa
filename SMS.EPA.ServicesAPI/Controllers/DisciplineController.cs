namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class DisciplineController : ApiController
    {
         private readonly IServiceAPI service;

        public DisciplineController() : this(null)
        {
        }

        public DisciplineController(IServiceAPI service)
        {
            this.service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
         {
             var criteria = new GetLastModifiedDateForDisciplineCriteria();

             DateTime fromDate = service.GetLastModifiedDate(criteria);

             return Ok(fromDate);
         }

         public IHttpActionResult GetFromDateDiscipline()
         {
             var criteria = new GetFromDateForDisciplineCriteria();

             DateTime fromDate = service.GetLastModifiedDate(criteria);

             return Ok(fromDate);
         }

         public IHttpActionResult Update(Discipline[] disciplines)
         {
             var criteria = new UpdateDisciplineCriteria()
             {
                 Disciplines = disciplines
             };

             CommandResult result = service.Update(criteria);

             return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
         }
    }
}
