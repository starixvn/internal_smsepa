namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;

    public interface ITokenAPI
    {
        TokenAccess Request(ApplicationInfo info);
    }
}
