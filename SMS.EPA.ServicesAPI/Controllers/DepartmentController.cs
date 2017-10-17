namespace SMS.EPA.ServicesAPI.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core.Model;
    using Criteria.Department;
    using Model.API;
    using Services;

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

        public async Task<IHttpActionResult> Update(Department[] departments)
        {
            var criteria = new UpdateDepartmentCriteria
            {
                Departments = departments
            };

            CommandResult result = await _service.Update(criteria).ConfigureAwait(false);
            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }

        public async Task<IHttpActionResult> GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForDepartmentCriteria();

            DateTime modifiedDate = await _service.GetLastModifiedDate(criteria).ConfigureAwait(false);

            return Ok(modifiedDate);
        }
    }
}