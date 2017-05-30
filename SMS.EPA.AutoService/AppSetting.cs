namespace SMS.EPA.AutoService
{
    using System;
    using System.Configuration;

    public static class AppSetting
    {
        public static bool SendMailEndPeriod => Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailEndPeriod"]);

        public static int TimerInterval => Convert.ToInt32(ConfigurationManager.AppSettings["TimerInterval"]);

        public static double HoursToStartService => Convert.ToDouble(ConfigurationManager.AppSettings["HoursToStartService"]);

        public static bool SendMailAllWhenPeriodStarted => Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailAllWhenPeriodStarted"]);

        public static bool EnableMonthlyPeriod => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableMonthlyPeriod"]);

        public static int StartDayCreateMonthlyPeriod => Convert.ToInt32(ConfigurationManager.AppSettings["StartDayCreateMonthlyPeriod"]);

        public static string RelativeEmailPath => ConfigurationManager.AppSettings["RelativeEmailPath"];

        public static bool IsRunAtStartService => Convert.ToBoolean(ConfigurationManager.AppSettings["IsRunAtStartService"]);
    }
}