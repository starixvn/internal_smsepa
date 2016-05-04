namespace SMS.EPA.ClientHRM.Services
{
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;

    public class TokenService : ITokenService
    {
        private readonly ITokenAPI _tokenAPI;

        public TokenService(ITokenAPI tokenAPI)
        {
            _tokenAPI = tokenAPI;
        }

        public TokenAccess Request(ApplicationInfo info)
        {
            return _tokenAPI.Request(info);
        }
    }
}
