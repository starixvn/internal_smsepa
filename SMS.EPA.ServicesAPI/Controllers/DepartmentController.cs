namespace SMS.EPA.ServicesAPI.Controllers
{
    using Core.Model;
    using Criteria.Department;
    using Model;
    using Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class DepartmentController : ApiController
    {
        private readonly IServiceAPI _service;

        public DepartmentController()
            : this(null)
        {
        }

        public DepartmentController(IServiceAPI employeeService)
        {
            _service = employeeService ?? new ServiceAPI();
        }

        public IHttpActionResult Update(Department[] departments)
        {
            var criteria = new UpdateDepartmentCriteria
            {
                Departments = departments
            };

            CommandResult result = _service.Update(criteria);
            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForDepartmentCriteria();

            DateTime modifiedDate = _service.GetLastModifiedDate(criteria);

            return Ok(modifiedDate);
        }
    }
}