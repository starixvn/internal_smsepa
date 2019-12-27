namespace SMS.EPA.ClientHrm.Services
{
    using System.Threading.Tasks;
    using Model;
    using ThirdPartyAPI;

    public class TokenService : ITokenService
    {
        private readonly ITokenAPI _tokenApi;

        public TokenService(ITokenAPI tokenApi)
        {
            _tokenApi = tokenApi;
        }

        public async Task<TokenAccess> Request(ApplicationInfo info)
        {
            return await _tokenApi.RequestAsync(info);
        }
    }
}