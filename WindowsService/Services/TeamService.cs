namespace SMS.EPA.WindowsService.Services
{
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThirdPartyAPI;

    public class TeamService : ITeamService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ITeamAPI teamApi;

        public TeamService(IDynamicRepository dynamicRepository, ITeamAPI teamApi)
        {
            this.dynamicRepository = dynamicRepository;
            this.teamApi = teamApi;
        }

        public IEnumerable<Team> GetTeams(DateTime? fromDate)
        {
            var criteria = new GetTeamCriteria { FromDate = fromDate };
            IEnumerable<Team> teams = dynamicRepository.Fetch<Team>(criteria);

            return teams;
        }

        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            return await teamApi.GetLastModifiedDate(token);
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<Team> teams, TokenAccess token)
        {
            return await teamApi.UpdateAsync(teams, token);
        }
    }
}