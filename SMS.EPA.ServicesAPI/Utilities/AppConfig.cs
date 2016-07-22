namespace SMS.EPA.ServicesAPI.Utilities
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public static class AppConfig
    {
        public static int TokenTimeOut
        {
            get
            {
                string timeout = ConfigurationManager.AppSettings["TokenTimeOut"];

                return string.IsNullOrEmpty(timeout) ? 1 : int.Parse(timeout);
            }
        }

        public static List<string> SupportedIPs
        {
            get
            {
                string ips = ConfigurationManager.AppSettings["SupportedIP"];

                return string.IsNullOrEmpty(ips) ? new List<string>() : ips.Split(';').ToList();
            }
        }

        public static List<string> PartnerIds
        {
            get
            {
                string partnerId = ConfigurationManager.AppSettings["PartnerId"];

                return string.IsNullOrEmpty(partnerId) ? new List<string>() : partnerId.Split(';').ToList();
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