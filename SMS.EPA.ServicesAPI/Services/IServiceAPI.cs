namespace SMS.EPA.ServicesAPI.Services
{
    using System;
    using Core.Model;
    using Fanex.Data.Repository;

    public interface IServiceAPI
    {
        DateTime GetLastModifiedDate(ICriteria criteria);

        CommandResult Update(ICriteria criteria);
    }
}