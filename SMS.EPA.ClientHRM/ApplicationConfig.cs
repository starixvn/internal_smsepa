namespace SMS.EPA.ClientHrm
{
    using System.Configuration;
    using SMS.EPA.ClientHrm.Model;

    public static class ApplicationConfig
    {
        public static double Interval
        {
            get
            {
                var interval = ConfigurationManager.AppSettings["Interval"].ToString();
                return double.Parse(interval) * 1000;
            }
        }

        public static string ServiceAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceAPI"];
            }
        }

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
    }
}