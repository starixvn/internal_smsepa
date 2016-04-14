using Fanex.Data.Repository;
using SMS.EPA.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.ServicesAPI.Services
{
    public interface IServiceAPI
    {
        DateTime GetMaxModifiedDate(ICriteria criteria);

        CommandResult Update(ICriteria criteria);
    }
}
