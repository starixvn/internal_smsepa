namespace SMS.EPA.AutoService
{
    using System;
    using System.Configuration;

    public static class AppSetting
    {
        public static bool SendMailEndPeriod
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailEndPeriod"]);
            }
        }

        public static bool SendMailAllWhenPeriodStarted
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailAllWhenPeriodStarted"]);
            }
        }

        public static bool EnableMonthlyPeriod
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableMonthlyPeriod"]);
            }
        }

        public static int StartDayCreateMonthlyPeriod
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["StartDayCreateMonthlyPeriod"]);
            }
        }
    }
}