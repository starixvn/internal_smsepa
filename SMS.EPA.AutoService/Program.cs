namespace SMS.EPA.AutoService
{
    using System;
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using Fanex.Logging;
    using Service.Implement;
    using Service.Interfaces;

    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DbSettingProviderManager
                .StartNewSession()
                .UseDefaultConnectionStringProvider()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider(
                    resourcePath: "~/",
                    enableWatching: true)
                .Run();

                IEPAAutoService service = new EPAAutoService();
                service.Run();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    LogService.Error(ex.InnerException.Message, ex);
                }
            }
        }
    }
}