namespace SMS.EPA.ServicesAPI.Services
{
    using Core.Model;
    using Core.Services;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using System;
    using System.Threading.Tasks;

    public class ServiceAPI : BaseService, IServiceAPI
    {
        public async Task<CommandResult> Update(ICriteria criteria)
        {
            LogService.Info(criteria.GetSettingKey());
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = await Repository.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            }

            return commandResult;
        }

        public async Task<DateTime> GetLastModifiedDate(ICriteria criteria)
        {
            var modifiedDate = await GetDataEntityAsync<DateTime>(criteria).ConfigureAwait(false);

            return modifiedDate;
        }
    }
}