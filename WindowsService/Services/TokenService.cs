namespace SMS.EPA.WindowsService.Services
{
    using System.Threading.Tasks;
    using Model;
    using ThirdPartyAPI;

    public class TokenService : ITokenService
    {
        private readonly ITokenAPI tokenApi;

        public TokenService(ITokenAPI tokenApi)
        {
            this.tokenApi = tokenApi;
        }

        public async Task<TokenAccess> Request(ApplicationInfo info)
        {
            return await tokenApi.RequestAsync(info);
        }
    }
}