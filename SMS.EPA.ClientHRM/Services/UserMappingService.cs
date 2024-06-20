namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Services;
    using SMS.EPA.ClientHrm.Criteria;
    using SMS.EPA.ClientHRM.Criteria;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.API;

    public class UserMappingService : BaseGOHttpClient, IUserMappingService
    {
        private readonly IBaseService baseService;

        public UserMappingService(
            IBaseService baseService,
            ICryptoService cryptoService) : base(cryptoService)
        {
            this.baseService = baseService;
        }

        public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var criteria = new GetLastModifiedDateUserMappingCriteria();
            return await baseService.GetDataEntityAsync<DateTime?>(criteria);
        }

        public async Task<IEnumerable<UserMapping>> GetUserMappingAsync(DateTime? lastModifiedDate)
        {
            await GetTokenAsync();
            if (!_jwttoken.IsExpired)
            {
                var modifiedDate = lastModifiedDate.HasValue ? lastModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null;
                var requestUrl = string.Format("api/UserMapping/GetBySiteId?lastModifiedDate={0}", modifiedDate);
                return await GetAsync<IEnumerable<UserMapping>>(requestUrl, _jwttoken.Token);
            }

            return new List<UserMapping>();
        }

        public async Task<CommandResult> UpdateUserMappingAsync(IEnumerable<UserMapping> userMappings)
        {
            var criteria = new UpdateUserMappingCriteria
            {
                UserMappings = userMappings
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}