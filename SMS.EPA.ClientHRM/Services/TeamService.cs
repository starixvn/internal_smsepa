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
        private readonly IDynamicRepository _dynamicRepository;
        private readonly ITeamAPI _teamAPI;

        public TeamService() : this(null, null) 
        {
        }

        public TeamService(IDynamicRepository dynamicRepository, ITeamAPI teamAPI)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _teamAPI = teamAPI ?? new TeamAPI();
        }

        public IEnumerable<Team> GetTeams(DateTime? fromDate)
        {
            var criteria = new GetTeamCriteria { FromDate = fromDate };
            IEnumerable<Team> teams = _dynamicRepository.Fetch<Team>(criteria);

            return teams;
        }

        public DateTime GetMaxModifiedDate(TokenAccess token)
        {
            return _teamAPI.GetMaxModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Team> teams, TokenAccess token)
        {
            return _teamAPI.Update(teams, token);
        }
    }
}
