namespace SMS.EPA.ServicesAPI.Controllers
{
    using Core.Model;
    using Criteria.EmployeeDisciplinePermission;
    using Model.API;
    using Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class EmployeeDisciplinePermissionController : ApiController
    {
        private readonly IServiceAPI _service;

        public EmployeeDisciplinePermissionController() : this(null) 
        { 
        }

        public EmployeeDisciplinePermissionController(IServiceAPI service)
        {
            _service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForEmployeeDisciplinePermissionCriteria();

            DateTime fromDate = _service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(EmployeeDisciplinePermission[] disciplines)
        {
            var criteria = new UpdateEmployeeSupervisorCriteria
            {
                EmployeeDisciplines = disciplines
            };

            CommandResult result = _service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
