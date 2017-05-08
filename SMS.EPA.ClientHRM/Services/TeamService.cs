namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using ThirdPartyAPI;

    public class TeamService : ITeamService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly ITeamAPI _teamApi;

        public TeamService(IDynamicRepository dynamicRepository, ITeamAPI teamApi)
        {
            _dynamicRepository = dynamicRepository;
            _teamApi = teamApi;
        }

        public IEnumerable<Team> GetTeams(DateTime? fromDate)
        {
            var criteria = new GetTeamCriteria { FromDate = fromDate };
            IEnumerable<Team> teams = _dynamicRepository.Fetch<Team>(criteria);

            return teams;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return _teamApi.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Team> teams, TokenAccess token)
        {
            return _teamApi.Update(teams, token);
        }
    }
}