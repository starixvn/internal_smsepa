namespace SMS.EPA.ServicesAPI.Controllers
{
    using Core.Model;
    using Criteria.Employee;
    using Model;
    using Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class EmployeeController : ApiController
    {
        private readonly IServiceAPI _service;

        public EmployeeController() : this(null)
        { 
        }

        public EmployeeController(IServiceAPI service)
        {
            _service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForEmployeeCriteria();

            DateTime fromDate = _service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(Employee[] employees)
        {
            var criteria = new UpdateEmployeeCriteria()
            {
                Employees = employees
            };

            CommandResult result = _service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
