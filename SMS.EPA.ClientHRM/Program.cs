using Fanex.Data.Configuration;
using System;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;

namespace SMS.EPA.ClientHRM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DbSettingProviderManager.Current.Start();
            ServicePointManager.ServerCertificateValidationCallback = delegate(
                object obj,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors errors)
                {
                    return true;
                };

            var servicesToRun = new ServiceBase[] 
            { 
                new WinService() 
            };

            if (Environment.UserInteractive)
            {
                var type = typeof(ServiceBase);
                const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var method = type.GetMethod("OnStart", Flags);
                foreach (var service in servicesToRun)
                {
                    method.Invoke(service, new object[] { args });
                }

                Console.WriteLine("Press any key to exit!");
                Console.ReadLine();

                foreach (var service in servicesToRun)
                {
                    service.Stop();
                }
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
