using SMS.EPA.ClientHRM.Model;
using System.Configuration;

namespace SMS.EPA.ClientHRM
{
    public static class ApplicationConfig
    {
        public static string WindowServiceName
        {
            get
            {
                return ConfigurationManager.AppSettings["WindowServiceName"];
            }
        }

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
    }
}
