namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.EmployeeDisciplinePermission;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class EmployeeDisciplinePermissionController : ApiController
    {
        private readonly IServiceAPI service;

        public EmployeeDisciplinePermissionController() : this(null) 
        { 
        }

        public EmployeeDisciplinePermissionController(IServiceAPI service)
        {
            this.service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForEmployeeDisciplinePermissionCriteria();

            DateTime fromDate = service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(EmployeeDisciplinePermission[] disciplines)
        {
            var criteria = new UpdateEmployeeSupervisorCriteria()
            {
                EmployeeDisciplines = disciplines
            };

            CommandResult result = service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
