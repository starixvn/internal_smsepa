namespace SMS.EPA.ClientHrm.Services
{
    using Model;

    public interface ITokenService
    {
        TokenAccess Request(ApplicationInfo info);
    }
}