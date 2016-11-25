namespace SMS.EPA.ClientHrm.Model
{
    public class ApplicationInfo
    {
        private const string GRANTTYPE = "password";

        public string GrantType
        {
            get { return GRANTTYPE; }
        }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}