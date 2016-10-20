namespace SMS.EPA.AutoGenerateEvaluation
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

        public static int TimerInterval
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["TimerInterval"]);
            }
        }

        public static double HoursToStartService
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["HoursToStartService"]);
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