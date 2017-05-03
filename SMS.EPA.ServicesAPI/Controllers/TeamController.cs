namespace SMS.EPA.ServicesAPI.Controllers
{
    using Core.Model;
    using Criteria.Team;
    using Model;
    using Services;
    using System;
    using System.Net;
    using System.Web.Http;

    [Authorize]
    public class TeamController : ApiController
    {
        private readonly IServiceAPI _service;

        public TeamController() : this(null) 
        {
        }

        public TeamController(IServiceAPI service)
        {
            _service = service ?? new ServiceAPI();
        }

        public IHttpActionResult GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForTeamCriteria();

            DateTime fromDate = _service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(Team[] teams)
        {
            var criteria = new UpdateTeamCriteria
            {
                Teams = teams
            };

            CommandResult result = _service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}
