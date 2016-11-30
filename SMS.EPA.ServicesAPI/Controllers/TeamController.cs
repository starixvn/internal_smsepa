namespace SMS.EPA.ServicesAPI.Controllers
{
    using Fanex.Logging;
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.Team;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class TeamController : ApiController
    {
        private readonly IServiceAPI service;

        public TeamController() : this(null) 
        {
        }

        public TeamController(IServiceAPI service)
        {
            this.service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForTeamCriteria();

            DateTime fromDate = service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(Team[] teams)
        {
            LogService.Info("update Team log by server");

            var criteria = new UpdateTeamCriteria()
            {
                Teams = teams
            };

            CommandResult result = service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
