using Fanex.Data.Repository;
using SMS.EPA.Core.Model;
using System;

namespace SMS.EPA.ServicesAPI.Services
{
    public interface IServiceAPI
    {
        DateTime GetLastModifiedDate(ICriteria criteria);

        CommandResult Update(ICriteria criteria);
    }
}
