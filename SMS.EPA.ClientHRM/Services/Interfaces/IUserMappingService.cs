namespace SMS.EPA.ClientHrm.Services
{
    using System.Threading.Tasks;
    using System;
    using SMS.EPA.Model.API;
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;

    public interface IUserMappingService
    {
        Task<DateTime?> GetLastUCModifiedDateAsync();

        Task<IEnumerable<UserMapping>> GetUserMapping(string requestUrl, DateTime? lastUCModifiedDate);

        Task<CommandResult> UpdateUserMapping(IEnumerable<UserMapping> userMappings);
    }
}
