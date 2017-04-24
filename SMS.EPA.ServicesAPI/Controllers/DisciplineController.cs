namespace SMS.EPA.ServicesAPI.Controllers
{
    using Core.Model;
    using Criteria.EmployeeDiscipline;
    using Model;
    using Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class DisciplineController : ApiController
    {
         private readonly IServiceAPI _service;

        public DisciplineController() : this(null)
        {
        }

        public DisciplineController(IServiceAPI service)
        {
            _service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
         {
             var criteria = new GetLastModifiedDateForDisciplineCriteria();

             DateTime fromDate = _service.GetLastModifiedDate(criteria);

             return Ok(fromDate);
         }

         public IHttpActionResult GetFromDateDiscipline()
         {
             var criteria = new GetFromDateForDisciplineCriteria();

             DateTime fromDate = _service.GetLastModifiedDate(criteria);

             return Ok(fromDate);
         }

         public IHttpActionResult Update(Discipline[] disciplines)
         {
             var criteria = new UpdateDisciplineCriteria()
             {
                 Disciplines = disciplines
             };

             CommandResult result = _service.Update(criteria);

             return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
         }
    }
}
