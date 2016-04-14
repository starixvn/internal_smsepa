namespace SMS.EPA.ServicesAPI.Services
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.ServicesAPI.Criteria;
    using SMS.EPA.ServicesAPI.Criteria.Department;
    using System;

    public class ServiceAPI : BaseService, IServiceAPI
    {
        public CommandResult Update(ICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public DateTime GetMaxModifiedDate(ICriteria criteria)
        {
            var modifiedDate = GetDataEntity<DateTime>(criteria);

            return modifiedDate;
        }
    }
}