namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System.Threading.Tasks;
    using Model;

    public interface ITokenAPI
    {
        Task<TokenAccess> RequestAsync(ApplicationInfo info);
    }
}