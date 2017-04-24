namespace SMS.EPA.ServicesAPI.Controllers
{
    using Core.Model;
    using Criteria.TeamDisciplinePermission;
    using Model;
    using Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class TeamDisciplinePermissionController : ApiController
    {
        private readonly IServiceAPI _service;

        public TeamDisciplinePermissionController() : this(null) 
        { 
        }

        public TeamDisciplinePermissionController(IServiceAPI service)
        {
            _service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForTeamDisciplinePermissionCriteria();

            DateTime fromDate = _service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(TeamDisciplinePermission[] disciplines)
        {
            var criteria = new UpdateTeamDisciplinePermissionCriteria()
            {
                EmployeeDisciplines = disciplines
            };

            CommandResult result = _service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
