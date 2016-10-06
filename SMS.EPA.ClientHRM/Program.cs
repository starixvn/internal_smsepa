namespace SMS.EPA.ClientHRM
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceProcess;
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using SMS.EPA.ClientHRM.Services;

    public class Program
    {
        public static void Main(string[] args)
        {
            DbSettingProviderManager
            .StartNewSession()
            .Use(new OldConnectionStringProvider())
            .UseDefaultDbSettingProvider(
                resourcePath: "~/",
                ignoreRedundantParameters: false,
                enableWatching: true)
            .Run();

            ServicePointManager.ServerCertificateValidationCallback = delegate(
                object obj,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors errors)
                {
                    return true;
                };

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

                Console.WriteLine("Press any key to exit!");
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
    }
}