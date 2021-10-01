namespace SMS.EPA.ServicesAPI.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Newtonsoft.Json;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.API;
    using SMS.EPA.ServicesAPI.Criteria.Employee;
    using SMS.EPA.ServicesAPI.Services;

    [Authorize]
    public class EmployeeController : ApiController
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EmployeeController));
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
            DateTime fromDate = await _service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Update(Employee[] employees)
        {
            var criteria = new UpdateEmployeeCriteria
            {
                Employees = employees
            };
            CommandResult result = await _service.Update(criteria);
            if (!result.IsSuccessful)
            {
                logger.Info(string.Format("Error: {0}\r\nEmployees: {1}", result.Message, JsonConvert.SerializeObject(employees)));
            }

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}