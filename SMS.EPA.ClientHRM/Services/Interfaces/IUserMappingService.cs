namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.API;

    public interface IUserMappingService
    {
        Task<DateTime?> GetLastModifiedDateAsync();

        Task<IEnumerable<UserMapping>> GetUserMappingAsync(DateTime? lastModifiedDate);

        Task<CommandResult> UpdateUserMappingAsync(IEnumerable<UserMapping> userMappings);
    }
}