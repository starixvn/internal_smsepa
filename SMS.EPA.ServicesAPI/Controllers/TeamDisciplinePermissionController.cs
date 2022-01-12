namespace SMS.EPA.ServicesAPI.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core.Model;
    using Criteria.TeamDisciplinePermission;
    using Model.API;
    using Services;

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

        public async Task<IHttpActionResult> GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForTeamDisciplinePermissionCriteria();
            DateTime fromDate = await _service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Update(TeamDisciplinePermission[] disciplines)
        {
            var criteria = new UpdateTeamDisciplinePermissionCriteria
            {
                EmployeeDisciplines = disciplines
            };
            CommandResult result = await _service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}