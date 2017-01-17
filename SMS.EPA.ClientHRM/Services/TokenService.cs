namespace SMS.EPA.ClientHrm.Services
{
    using SMS.EPA.ClientHrm.Model;
    using SMS.EPA.ClientHrm.ThirdPartyAPI;

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