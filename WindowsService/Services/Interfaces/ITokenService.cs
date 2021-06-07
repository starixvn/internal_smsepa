namespace SMS.EPA.WindowsService.Services
{
    using System.Threading.Tasks;
    using Model;

    public interface ITokenService
    {
        Task<TokenAccess> Request(ApplicationInfo info);
    }
}