namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using SMS.EPA.ClientHrm.Model;

    public interface ITokenAPI
    {
        TokenAccess Request(ApplicationInfo info);
    }
}