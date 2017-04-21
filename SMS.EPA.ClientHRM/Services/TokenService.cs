namespace SMS.EPA.ClientHrm.Services
{
    using Model;
    using ThirdPartyAPI;

    public class TokenService : ITokenService
    {
        private readonly ITokenAPI _tokenApi;

        public TokenService(ITokenAPI tokenApi)
        {
            _tokenApi = tokenApi;
        }

        public TokenAccess Request(ApplicationInfo info)
        {
            return _tokenApi.Request(info);
        }
    }
}