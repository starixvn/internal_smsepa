namespace SMS.EPA.AutoService
{
    using System;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using SMS.EPA.AutoService.Service.Implement;
    using SMS.EPA.AutoService.Service.Interfaces;

    public static class Program
    {
        public static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                DbSettingProviderManager
                    .StartNewSession()
                    .WithSqlServer("~/SpNames.xml")
                    .Run();

                IEPAAutoService service = new EPAAutoService();
                service.Run();
            }
            catch (Exception ex)
            {
                log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    logger.Error(ex.Message, ex);
                }

                if (ex.InnerException != null)
                {
                    logger.Error(ex.InnerException.Message, ex);
                }
            }
        }
    }
}