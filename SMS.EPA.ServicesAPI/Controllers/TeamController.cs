namespace SMS.EPA.ServicesAPI.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core.Model;
    using Criteria.Team;
    using Model.API;
    using Services;

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

        public async Task<IHttpActionResult> GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForTeamCriteria();
            DateTime fromDate = await _service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Update(Team[] teams)
        {
            var criteria = new UpdateTeamCriteria
            {
                Teams = teams
            };
            CommandResult result = await _service.Update(criteria);

            return result.IsSuccessful ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }
    }
}