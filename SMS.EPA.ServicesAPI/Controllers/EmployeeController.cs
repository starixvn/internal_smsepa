namespace SMS.EPA.ServicesAPI.Controllers
{
    using Fanex.Logging;
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.Employee;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class EmployeeController : ApiController
    {
        private readonly IServiceAPI service;

        public EmployeeController() : this(null)
        { 
        }

        public EmployeeController(IServiceAPI service)
        {
            this.service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForEmployeeCriteria();

            DateTime fromDate = service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(Employee[] employees)
        {
            LogService.Info("update employee log by server");

            var criteria = new UpdateEmployeeCriteria()
            {
                Employees = employees
            };

            CommandResult result = service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
