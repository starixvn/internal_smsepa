// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/26/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation
{
    using System.ServiceProcess;
    using Fanex.Data.Configuration;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            DbSettingProviderManager.Current.Start();
            log4net.Config.XmlConfigurator.Configure();

            ServiceBase[] _servicesToRun;
            _servicesToRun = new ServiceBase[]
            { 
                new AutoService() 
            };
            ServiceBase.Run(_servicesToRun);
        }
    }
}