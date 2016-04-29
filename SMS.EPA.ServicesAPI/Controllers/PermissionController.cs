namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.Permission;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    public class PermissionController : ApiController
    {
        private readonly IServiceAPI _permissionService;

        public PermissionController() : this(null) 
        { 
        }

        public PermissionController(IServiceAPI permissionService)
        {
            _permissionService = permissionService ?? new ServiceAPI();
        }

        public IHttpActionResult GetMaxModifiedDateForEmployeeDiscipline()
        {
            var criteria = new GetMaxModifiedDateForEmployeeDisciplineCriteria();

            DateTime fromDate = _permissionService.GetMaxModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult UpdateEmployeeDiscipline(EmployeeDisciplinePermission[] disciplines)
        {
            var criteria = new UpdateEmployeeDisciplineCriteria()
            {
                EmployeeDisciplines = disciplines
            };

            CommandResult result = _permissionService.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }

        public IHttpActionResult GetMaxModifiedDateForTeamDiscipline()
        {
            var criteria = new GetMaxModifiedDateForTeamDisciplineCriteria();

            DateTime fromDate = _permissionService.GetMaxModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult UpdateTeamDiscipline(TeamDisciplinePermission[] disciplines)
        {
            var criteria = new UpdateTeamDisciplineCriteria()
            {
                EmployeeDisciplines = disciplines
            };

            CommandResult result = _permissionService.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
