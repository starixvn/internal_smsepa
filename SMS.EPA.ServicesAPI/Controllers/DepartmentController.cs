namespace SMS.EPA.ServicesAPI.Controllers
{
    using Fanex.Logging;
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.Department;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class DepartmentController : ApiController
    {
        private readonly IServiceAPI service;

        public DepartmentController()
            : this(null)
        {
        }

        public DepartmentController(IServiceAPI employeeService)
        {
            this.service = employeeService ?? new ServiceAPI();
        }

        public IHttpActionResult Update(Department[] departments)
        {
            LogService.Info("update department log by server");

            var criteria = new UpdateDepartmentCriteria
            {
                Departments = departments
            };

            CommandResult result = service.Update(criteria);
            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForDepartmentCriteria();

            DateTime modifiedDate = service.GetLastModifiedDate(criteria);

            return Ok(modifiedDate);
        }
    }
}