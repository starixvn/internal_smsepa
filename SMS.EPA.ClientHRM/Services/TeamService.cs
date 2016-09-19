namespace SMS.EPA.ClientHRM.Services
{
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHRM.Criteria;
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;
    using System;
    using System.Collections.Generic;

    public class TeamService : ITeamService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ITeamAPI teamAPI;

        public TeamService(IDynamicRepository dynamicRepository, ITeamAPI teamAPI)
        {
            this.dynamicRepository = dynamicRepository;
            this.teamAPI = teamAPI;
        }

        public IEnumerable<Team> GetTeams(DateTime? fromDate)
        {
            var criteria = new GetTeamCriteria { FromDate = fromDate };
            IEnumerable<Team> teams = dynamicRepository.Fetch<Team>(criteria);

            return teams;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return teamAPI.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Team> teams, TokenAccess token)
        {
            return teamAPI.Update(teams, token);
        }
    }
}
