namespace SMS.EPA.AutoJob.Service
{
    using System.Threading.Tasks;

    public interface IEPAUserMappingService
    {
        Task SyncDataUserMapping();
    }
}