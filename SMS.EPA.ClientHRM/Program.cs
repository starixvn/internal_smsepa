namespace SMS.EPA.ClientHrm
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.ServiceProcess;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Services;

    public static class Program
    {
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                DbSettingProviderManager
                    .StartNewSession()
                    .WithSqlServer("~/App_Data/SpNames.xml")
                    .Run();

                ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;

                Bootstrap.Start();

                var servicesToRun = new ServiceBase[]
                {
                    new WinService(Bootstrap.Container.GetInstance<ISyncEPADataService>(),
                        Bootstrap.Container.GetInstance<IUserMappingService>(),
                        Bootstrap.Container.GetInstance<IUnitHeadService>())
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
                        Console.WriteLine("Stop {0}", service.ServiceName);
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
                log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));
                logger.Error(ex.Message, ex);
            }
        }
    }
}