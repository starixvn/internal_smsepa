namespace SMS.EPA.ClientHrm.Model
{
    public class ApplicationInfo
    {
        public string GrantType => "password";

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}