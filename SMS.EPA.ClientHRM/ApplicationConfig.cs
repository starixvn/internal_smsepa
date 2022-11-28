namespace SMS.EPA.ClientHrm
{
    using Model;
    using System.Configuration;

    public static class ApplicationConfig
    {
        const int milisecondUnit = 1000;

        public static double Interval
        {
            get
            {
                var interval = ConfigurationManager.AppSettings["Interval"];
                return double.Parse(interval) * milisecondUnit;
            }
        }

        public static string ServiceAPI => ConfigurationManager.AppSettings["ServiceAPI"];

        public static ApplicationInfo ApplicationInfo
        {
            get
            {
                var applicationId = ConfigurationManager.AppSettings["ApplicationId"];

                return new ApplicationInfo { UserName = applicationId, Password = applicationId };
            }
        }

        public static bool IsWriteLog
        {
            get
            {
                string isWriteLog = ConfigurationManager.AppSettings["IsWriteLog"];

                return bool.Parse(isWriteLog);
            }
        }

        public static string PhotoPath
        {
            get
            {
                string photoPath = ConfigurationManager.AppSettings["PhotoPath"];

                return photoPath;
            }
        }

        public static short UserControlSettings_SiteId
            => short.Parse(ConfigurationManager.AppSettings["UserControl.Configurations:SiteId"]);

        public static string UserControlSettings_AccessToken
            => ConfigurationManager.AppSettings["UserControl.Configurations:AccessToken"];

        public static string UserControlSettings_ServiceUrl
            => ConfigurationManager.AppSettings["UserControl.Configurations:ServiceUrl"];
    }
}