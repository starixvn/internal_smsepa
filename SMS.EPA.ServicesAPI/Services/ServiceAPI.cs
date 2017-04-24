namespace SMS.EPA.ServicesAPI.Services
{
    using Core.Model;
    using Core.Services;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using System;

    public class ServiceAPI : BaseService, IServiceAPI
    {
        public CommandResult Update(ICriteria criteria)
        {
            LogService.Info(criteria.GetSettingKey());
            var commandResult = new CommandResult();

            if (criteria.IsValid())
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