namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.Team;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    public class TeamController : ApiController
    {
        private readonly IServiceAPI _teamService;

        public TeamController(IServiceAPI teamService)
        {
            _teamService = teamService ?? new ServiceAPI();
        }

        public IHttpActionResult GetMaxModifiedDate()
        {
            var criteria = new GetMaxModifiedDateForTeamCriteria();

            DateTime fromDate = _teamService.GetMaxModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(IList<Team> teams)
        {
            var criteria = new UpdateTeamCriteria()
            {
                Teams = teams
            };

            CommandResult result = _teamService.Update(criteria);

            return Ok(result);
        }
    }
}
