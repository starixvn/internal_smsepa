namespace SMS.EPA.AutoService
{
    using System;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using SharpRaven;
    using SMS.EPA.AutoService.Service;

    public static class Program
    {
        public static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                LogManager
                    .SetDefaultLogCategory(AppSetting.Sentry_Category)
                    .Use(new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(AppSetting.Sentry_Dsn),
                        WriteLogTimeoutSeconds = AppSetting.Sentry_Timeout
                    }));

                DbSettingProviderManager
                    .StartNewSession()
                    .WithSqlServer("~/SpNames.xml")
                    .Run();

                IEPAAutoService service = new EPAAutoService();
                service.Run();
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }
        }
    }
}