namespace SMS.EPA.AutoJob.Services
{
    using System.Threading.Tasks;

    public interface IUserMappingService
    {
        Task SyncUserMappingAsync();
    }
}