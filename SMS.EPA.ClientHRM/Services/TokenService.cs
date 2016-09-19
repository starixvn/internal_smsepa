namespace SMS.EPA.ClientHRM.Services
{
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;

    public class TokenService : ITokenService
    {
        private readonly ITokenAPI tokenAPI;

        public TokenService(ITokenAPI tokenAPI)
        {
            this.tokenAPI = tokenAPI;
        }

        public TokenAccess Request(ApplicationInfo info)
        {
            return tokenAPI.Request(info);
        }
    }
}
