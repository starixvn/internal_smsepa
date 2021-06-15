namespace SMS.EPA.WindowsService
{
    using Model;

    public class ApplicationConfig
    {
        public int Interval { get; set; }

        public static string ServiceAPI { get; set; }

        public static string ApplicationId { get; set; }

        public static ApplicationInfo ApplicationInfo
        {
            get
            {
                return new ApplicationInfo { UserName = ApplicationId, Password = ApplicationId };
            }
        }

        public static bool IsWriteLog { get; set; }

        public static string PhotoPath { get; set; }
    }
}