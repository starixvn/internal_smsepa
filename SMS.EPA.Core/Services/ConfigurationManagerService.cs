namespace SMS.EPA.Core.Services
{
    using System.Configuration;

    public static class ConfigurationManagerService
    {
        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}