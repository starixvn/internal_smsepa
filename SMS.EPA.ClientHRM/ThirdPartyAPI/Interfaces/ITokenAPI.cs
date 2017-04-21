namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using SMS.EPA.ClientHrm.Model;

    public interface ITokenAPI
    {
        TokenAccess Request(ApplicationInfo info);
    }
}