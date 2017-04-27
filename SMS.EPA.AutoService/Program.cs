namespace SMS.EPA.AutoService
{
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using System.ServiceProcess;

    public static class Program
    {
        public static void Main()
        {
            DbSettingProviderManager
            .StartNewSession()
            .Use(new OldConnectionStringProvider())
            .UseDefaultDbSettingProvider(
                resourcePath: "~/",
                enableWatching: true)
            .Run();
            log4net.Config.XmlConfigurator.Configure();

            var servicesToRun = new ServiceBase[]
            {
                new AutoService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}