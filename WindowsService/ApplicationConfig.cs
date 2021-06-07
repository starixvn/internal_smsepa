namespace SMS.EPA.WindowsService
{
    using Model;
    using System.Configuration;

    public class ApplicationConfig
    {

        public int Interval { get; set; }

        public static string ServiceAPI = "http://localhost:63238/";//{ get; set; }

        public static string ApplicationId = "aenfJVK95Xu2Znh8";//{ get; set; }

        public static ApplicationInfo ApplicationInfo
        {
            get
            {
                return new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            }
        }

        public bool IsWriteLog = true;//{ get; set; }

        public static string PhotoPath = "C:\\InternalWebs\\SMSEPA\\Local\\Avatars";//{ get; set; }
    }
}