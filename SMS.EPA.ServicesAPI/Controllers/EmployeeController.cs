namespace SMS.EPA.ServicesAPI.Controllers
{
    using Core.Model;
    using Criteria.Employee;
    using EPA.Model.API;
    using Services;
    using System;
    using System.Net;
    using System.Web.Http;
    using System.Threading.Tasks;

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

        public async Task<IHttpActionResult> GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForEmployeeCriteria();
            DateTime fromDate = await _service.GetLastModifiedDate(criteria).ConfigureAwait(false);

            return Ok(fromDate);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Update(Employee[] employees)
        {
            var criteria = new UpdateEmployeeCriteria
            {
                Employees = employees
            };
            CommandResult result = await _service.Update(criteria).ConfigureAwait(false);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
