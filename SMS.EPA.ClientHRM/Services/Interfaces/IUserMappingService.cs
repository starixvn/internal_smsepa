namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.API;

    public interface IUserMappingService
    {
        Task<DateTime?> GetLastUCModifiedDateAsync();

        Task<IEnumerable<UserMapping>> GetUserMapping(DateTime? lastUCModifiedDate);

        Task<CommandResult> UpdateUserMapping(IEnumerable<UserMapping> userMappings);
    }
}
