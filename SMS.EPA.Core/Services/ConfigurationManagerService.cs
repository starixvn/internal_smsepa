namespace SMS.EPA.Core.Services
{
    using System.Configuration;

    public class ConfigurationManagerService : IConfigurationManagerService
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}