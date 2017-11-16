namespace SMS.EPA.ServicesAPI.Services
{
    using System;
    using System.Threading.Tasks;
    using Core.Model;
    using Core.Services;
    using Fanex.Data.Repository;
    using Fanex.Logging;

    public class ServiceAPI : IServiceAPI
    {
        private readonly IBaseService baseService;

        public ServiceAPI(IBaseService baseService = null)
        {
            this.baseService = baseService ?? new BaseService();
        }

        public async Task<CommandResult> Update(ICriteria criteria)
        {
            LogService.Info(criteria.GetSettingKey());
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            }

            return commandResult;
        }

        public async Task<DateTime> GetLastModifiedDate(ICriteria criteria)
        {
            var modifiedDate = await baseService.GetDataEntityAsync<DateTime>(criteria).ConfigureAwait(false);

            return modifiedDate;
        }
    }
}