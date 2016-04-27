namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.Department;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    [Authorize]
    public class DepartmentController : ApiController
    {
        private readonly IServiceAPI _service;

        public DepartmentController() : this(null) 
        {
        }

        public DepartmentController(IServiceAPI employeeService)
        {
            _service = employeeService ?? new ServiceAPI();
        }

        [HttpPost]
        public IHttpActionResult Update([FromBody]Department[] departments)
        {
            var criteria = new UpdateDepartmentCriteria
            {
                Departments = departments
            };

            CommandResult result = _service.Update(criteria);

            return Ok(result);
        }

        public IHttpActionResult GetMaxModifiedDate()
        {
            var criteria = new GetMaxModifiedDateForDepartmentCriteria();

            DateTime modifiedDate = _service.GetMaxModifiedDate(criteria);

            return Ok(modifiedDate);
        }
    }
}