namespace WindowsService
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SMS.EPA.WindowsService;
    using SMS.EPA.WindowsService.Services;
    using System.Threading;
    using System.Threading.Tasks;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly ISynchronizeService service;
        private readonly ApplicationConfig settings;
        const int milisecondUnit = 1000;

        public Worker(ILogger<Worker> logger, IOptions<ApplicationConfig> settings, ISynchronizeService service)
        {
            this.logger = logger;
            this.service = service;
            this.settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Sync Data");
                await service.Synchronize();
                await Task.Delay(settings.Interval * milisecondUnit, stoppingToken);
            }
        }
    }
}
