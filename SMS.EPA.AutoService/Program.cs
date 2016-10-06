namespace SMS.EPA.AutoService
{
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using SMS.EPA.AutoService.Service;

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

            log4net.Config.XmlConfigurator.Configure();

            var service = new EPAService();
            service.Run();
        }
    }
}