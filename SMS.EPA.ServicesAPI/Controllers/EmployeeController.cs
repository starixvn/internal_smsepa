namespace SMS.EPA.ServicesAPI.Controllers
{
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
        private readonly IServiceAPI _employeeService;

        public EmployeeController() : this(null)
        { 
        }

        public EmployeeController(IServiceAPI employeeService)
        {
            _employeeService = employeeService ?? new ServiceAPI();
        }

        public IHttpActionResult GetMaxModifiedDate()
        {
            var criteria = new GetMaxModifiedDateForEmployeeCriteria();

            DateTime fromDate = _employeeService.GetMaxModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(Employee[] employees)
        {
            var criteria = new UpdateEmployeeCriteria()
            {
                Employees = employees
            };

            CommandResult result = _employeeService.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
