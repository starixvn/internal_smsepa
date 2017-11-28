namespace SMS.EPA.ClientHrm
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.ServiceProcess;
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using Fanex.Logging;
    using Services;

    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider(
                    resourcePath: "~/",
                    enableWatching: true)
                .Run();

                ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;

                Bootstrap.Start();

                var servicesToRun = new ServiceBase[]
                {
                new WinService(Bootstrap.Container.GetInstance<ISynchronizeService>())
                };

                if (Environment.UserInteractive)
                {
                    var type = typeof(ServiceBase);
                    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var method = type.GetMethod("OnStart", Flags);
                    foreach (var service in servicesToRun)
                    {
                        method.Invoke(service, new object[] { args });
                    }

                    Console.ReadLine();

                    foreach (var service in servicesToRun)
                    {
                        service.Stop();
                    }
                }
                else
                {
                    ServiceBase.Run(servicesToRun);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    LogService.Error(ex.Message, ex);
                }
            }
        }
    }
}