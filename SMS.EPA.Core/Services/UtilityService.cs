namespace SMS.EPA.Core.Services
{
    using System.IO;
    using System.Web;

    public class UtilityService : IUtilityService
    {
        public string GetAvatar(string code)
        {
            code += ".jpg";

            if (File.Exists(HttpContext.Current.Server.MapPath("/avatars/" + code)))
            {
                return "avatars/" + code;
            }

            return "avatars/avatar.jpg";
        }
    }
}