namespace SMS.EPA.AutoService
{
    using Fanex.Data.Configuration;
    using SMS.EPA.AutoService.Service;

    public class Program
    {
        public static void Main(string[] args)
        {
            DbSettingProviderManager.Current.Start();
            log4net.Config.XmlConfigurator.Configure();

            var service = new EPAService();
            service.Run();
        }
    }
}