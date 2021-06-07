namespace SMS.EPA.WindowsService.Services
{
    using System.Threading.Tasks;

    public interface ISynchronizeService
    {
        Task Synchronize();
    }
}