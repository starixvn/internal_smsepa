namespace SMS.EPA.AutoService
{
    using System;
    using System.Configuration;

    public static class AppSetting
    {
        public static bool SendMailEndPeriod => Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailEndPeriod"]);

        public static bool SendMailAllWhenPeriodStarted => Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailAllWhenPeriodStarted"]);

        public static bool EnableMonthlyPeriod => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableMonthlyPeriod"]);

        public static int StartDayCreateMonthlyPeriod => Convert.ToInt32(ConfigurationManager.AppSettings["StartDayCreateMonthlyPeriod"]);

        public static string RelativeEmailPath => ConfigurationManager.AppSettings["RelativeEmailPath"];

        public static bool ProbationExtendedQuestionEnabled => Convert.ToBoolean(ConfigurationManager.AppSettings["ProbationExtendedQuestionEnabled"]);

        public static string Sentry_Category => ConfigurationManager.AppSettings["Sentry:Category"];
        public static string Sentry_Dsn => ConfigurationManager.AppSettings["Sentry:Dsn"];
        public static int Sentry_Timeout => Convert.ToInt32(ConfigurationManager.AppSettings["Sentry:Timeout"]);
    }
}