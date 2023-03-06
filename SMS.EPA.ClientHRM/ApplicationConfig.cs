namespace SMS.EPA.ClientHrm
{
    using System.Configuration;
    using Model;

    public static class ApplicationConfig
    {
        public static int Interval
        {
            get
            {
                var interval = ConfigurationManager.AppSettings["Interval"];
                return int.Parse(interval);
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

        public static string PhotoPath
        {
            get
            {
                string photoPath = ConfigurationManager.AppSettings["PhotoPath"];

                return photoPath;
            }
        }

        public static bool IsEnableSyncUserMapping => bool.Parse(ConfigurationManager.AppSettings["IsEnableSyncUserMapping"]);
        public static bool IsEnableSyncUnitHead => bool.Parse(ConfigurationManager.AppSettings["IsEnableSyncUnitHead"]);

        public static string UserControl_Username => ConfigurationManager.AppSettings["UserControl.Username"];
        public static string UserControl_Password => ConfigurationManager.AppSettings["UserControl.Password"];

        public static short UserControlSettings_SiteId
            => short.Parse(ConfigurationManager.AppSettings["UserControl.Configurations:SiteId"]);

        public static string UserControlSettings_AccessToken
            => ConfigurationManager.AppSettings["UserControl.Configurations:AccessToken"];

        public static string UserControlSettings_ServiceUrl
            => ConfigurationManager.AppSettings["UserControl.Configurations:ServiceUrl"];
    }
}