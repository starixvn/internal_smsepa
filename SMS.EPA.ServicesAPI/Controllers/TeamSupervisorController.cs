namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.Permission;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    public class TeamSupervisorController : ApiController
    {
        private readonly IServiceAPI _permissionService;

        public TeamSupervisorController() : this(null) 
        { 
        }

        public TeamSupervisorController(IServiceAPI permissionService)
        {
            _permissionService = permissionService ?? new ServiceAPI();
        }

        public IHttpActionResult GetMaxModifiedDate()
        {
            var criteria = new GetMaxModifiedDateForTeamSupervisorCriteria();

            DateTime fromDate = _permissionService.GetMaxModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(TeamDisciplinePermission[] disciplines)
        {
            var criteria = new UpdateTeamSupervisorCriteria()
            {
                EmployeeDisciplines = disciplines
            };

            CommandResult result = _permissionService.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
