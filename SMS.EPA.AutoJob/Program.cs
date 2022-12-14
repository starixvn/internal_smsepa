namespace SMS.EPA.AutoJob
{
    using System;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public static class Program
    {
        public static void Main(string[] args)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));

            try
            {
                logger.Info("Application start.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception.Message, exception);
            }
            finally
            {
                logger.Logger.Repository.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://*:5000").UseStartup<Startup>();
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddLog4Net("log4net.config");
                });
    }
}
