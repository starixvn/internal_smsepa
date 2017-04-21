namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Model;

    public interface ITokenAPI
    {
        TokenAccess Request(ApplicationInfo info);
    }
}