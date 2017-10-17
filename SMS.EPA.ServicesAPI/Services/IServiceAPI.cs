namespace SMS.EPA.ServicesAPI.Services
{
    using System;
    using System.Threading.Tasks;
    using Core.Model;
    using Fanex.Data.Repository;

    public interface IServiceAPI
    {
        Task<DateTime> GetLastModifiedDate(ICriteria criteria);

        Task<CommandResult> Update(ICriteria criteria);
    }
}