namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.TeamDisciplinePermission;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class TeamDisciplinePermissionController : ApiController
    {
        private readonly IServiceAPI service;

        public TeamDisciplinePermissionController() : this(null) 
        { 
        }

        public TeamDisciplinePermissionController(IServiceAPI service)
        {
            this.service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForTeamDisciplinePermissionCriteria();

            DateTime fromDate = service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(TeamDisciplinePermission[] disciplines)
        {
            var criteria = new UpdateTeamDisciplinePermissionCriteria()
            {
                EmployeeDisciplines = disciplines
            };

            CommandResult result = service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
