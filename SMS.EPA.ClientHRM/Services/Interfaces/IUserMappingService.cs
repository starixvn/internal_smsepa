namespace SMS.EPA.ClientHrm.Services
{
    using System.Threading.Tasks;
    using System;
    using SMS.EPA.Model.API;
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;

    public interface IUserMappingService
    {
        Task<DateTime?> GetLastSyncUserDateAsync();

        Task<IEnumerable<UserMapping>> GetUserMapping(string requestUrl, DateTime? lastSyncDate);

        Task<CommandResult> UpdateUserMapping(IEnumerable<UserMapping> userMappings);
    }
}
