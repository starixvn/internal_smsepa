namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.EmployeeDisciplinePermission;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    public class EmployeeDisciplinePermissionController : ApiController
    {
        private readonly IServiceAPI _permissionService;

        public EmployeeDisciplinePermissionController() : this(null) 
        { 
        }

        public EmployeeDisciplinePermissionController(IServiceAPI permissionService)
        {
            _permissionService = permissionService ?? new ServiceAPI();
        }

        public IHttpActionResult GetMaxModifiedDate()
        {
            var criteria = new GetMaxModifiedDateForEmployeeDisciplinePermissionCriteria();

            DateTime fromDate = _permissionService.GetMaxModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(EmployeeDisciplinePermission[] disciplines)
        {
            var criteria = new UpdateEmployeeSupervisorCriteria()
            {
                EmployeeDisciplines = disciplines
            };

            CommandResult result = _permissionService.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
