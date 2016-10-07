namespace SMS.EPA.AutoGenerateEvaluation
{
    using System.ServiceProcess;
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;

    public static class Program
    {
        public static void Main()
        {
            DbSettingProviderManager
            .StartNewSession()
            .Use(new OldConnectionStringProvider())
            .UseDefaultDbSettingProvider(
                resourcePath: "~/",
                ignoreRedundantParameters: false,
                enableWatching: true)
            .Run();
            log4net.Config.XmlConfigurator.Configure();

            ServiceBase[] _servicesToRun;
            _servicesToRun = new ServiceBase[]
            {
                new AutoService()
            };
            ServiceBase.Run(_servicesToRun);
        }
    }
}