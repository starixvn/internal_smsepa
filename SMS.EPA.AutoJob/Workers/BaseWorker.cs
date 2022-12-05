namespace SMS.EPA.AutoJob.Workers
{
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using Microsoft.Extensions.Hosting;
    using SMS.EPA.AutoJob.Models;

    public abstract class BaseWorker : BackgroundService
    {
        protected JobSetting jobSetting;

        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(BaseWorker));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const byte MillisecondsDelay = 10;
            await Task.Delay(MillisecondsDelay).ConfigureAwait(false);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await RunJobAsync();

                    await Task.Delay(TimeSpan.FromSeconds(jobSetting.TimeInterval), stoppingToken);
                }

                logger.Info($"Execution ended. Cancelation token cancelled = {stoppingToken.IsCancellationRequested}");
            }
            catch (Exception ex)
            {
                logger.Info("Unhandeled exception. Execution Stopping");
                logger.Info(ex.Message);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.Info($"{jobSetting.JobName}: Service Starting");
            return base.StartAsync(cancellationToken);
        }

        private async Task RunJobAsync()
        {
            try
            {
                if (jobSetting.IsEnabled)
                {
                    logger.Warn($"{jobSetting.JobName}: Doing...");
                    await ExecuteJobAsync();
                    logger.Warn($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                logger.Error($"{jobSetting.JobName}: Error - {ex}");
            }
        }

        public abstract Task ExecuteJobAsync();

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.Error($"{jobSetting.JobName}: Stopping Service");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logger.Warn($"{jobSetting.JobName}: Disposing Service");
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}
