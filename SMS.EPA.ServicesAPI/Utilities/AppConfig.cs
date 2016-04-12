namespace SMS.EPA.ServicesAPI.Utilities
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public static class AppConfig
    {
        public static List<string> SupportedIP
        {
            get
            {
                var ips = ConfigurationManager.AppSettings["SupportedIP"];

                return ips == null ? new List<string>() : ips.Split(';').ToList();
            }
        }

        public static List<string> PartnerId
        {
            get
            {
                var partnerId = ConfigurationManager.AppSettings["PartnerId"];

                return partnerId == null? new List<string>():partnerId.Split(';').ToList();
            }
        }
    }
}