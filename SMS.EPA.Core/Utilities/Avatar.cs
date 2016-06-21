namespace SMS.EPA.Core.Utilities
{
    using System.IO;
    using System.Web;

    public static class Avatar
    {
        public static string GetAvatar(string code)
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