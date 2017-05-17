namespace SMS.EPA.ServicesAPI.Utilities
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public static class AppConfig
    {
        private static List<string> _supportedIps;
        private static List<string> _partnerIds;

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
                if (_supportedIps == null || !_supportedIps.Any())
                {
                    string ips = ConfigurationManager.AppSettings["SupportedIP"];
                    _supportedIps = string.IsNullOrEmpty(ips) ? new List<string>() : ips.Split(';').ToList();
                }

                return _supportedIps;
            }
        }

        public static List<string> PartnerIds
        {
            get
            {
                if (_partnerIds == null || !_partnerIds.Any())
                {
                    string partnerId = ConfigurationManager.AppSettings["PartnerId"];

                    _partnerIds = string.IsNullOrEmpty(partnerId) ? new List<string>() : partnerId.Split(';').ToList();
                }

                return _partnerIds;
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