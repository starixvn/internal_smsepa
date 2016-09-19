namespace SMS.EPA.ServicesAPI.Services
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using System;

    public class ServiceAPI : BaseService, IServiceAPI
    {
        public CommandResult Update(ICriteria criteria)
        {
            LogService.Info(criteria.GetSettingKey());
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public DateTime GetLastModifiedDate(ICriteria criteria)
        {
            var modifiedDate = GetDataEntity<DateTime>(criteria);

            return modifiedDate;
        }
    }
}