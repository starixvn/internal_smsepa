namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Model.Department;
    using SMS.EPA.ServicesAPI.Criteria;
    using SMS.EPA.ServicesAPI.Services;
    using System.Collections.Generic;
    using System.Web.Http;

    [Authorize]
    public class DepartmentController : ApiController
    {
        private readonly IDeparmentService _service;

        public DepartmentController()
        {
            _service = new DepartementService();
        }

        [HttpPost]
        public IHttpActionResult Insert(DepartmentModel department)
        {
            var criteria = new InsertAndUpdateDepartmentCriteria
            {
                Departments = new List<DepartmentModel>()
            };

            _service.Update(criteria);

            return Ok();
        }
    }
}