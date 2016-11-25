namespace SMS.EPA.ClientHrm.Services
{
    using SMS.EPA.ClientHrm.Model;

    public interface ITokenService
    {
        TokenAccess Request(ApplicationInfo info);
    }
}