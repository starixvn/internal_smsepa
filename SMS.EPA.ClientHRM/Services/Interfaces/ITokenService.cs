namespace SMS.EPA.ClientHRM.Services
{
    using SMS.EPA.ClientHRM.Model;

    public interface ITokenService
    {
        TokenAccess Request(ApplicationInfo info);
    }
}
