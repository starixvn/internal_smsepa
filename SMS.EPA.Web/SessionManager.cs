namespace SMS.EPA.Web
{
    using System;
    using System.Web;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.Authentication;

    public static class SessionManager
    {
        public static UserSession CurrentUser
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    return (UserSession)HttpContext.Current.Session[Constants.CurrentSessionKey];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                HttpContext.Current.Session[Constants.CurrentSessionKey] = value;
            }
        }

        public static void ClearAll()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                CurrentUser = null;
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
                HttpContext.Current.Response.Cookies.Clear();
            }
        }

        public static void Set<T>(string sessionKey, T value)
        {
            HttpContext.Current.Session[sessionKey] = value;
        }

        public static T Get<T>(string sessionKey)
        {
            try
            {
                T value = (T)HttpContext.Current.Session[sessionKey];
                return value;
            }
            catch (NullReferenceException)
            {
                return default(T);
            }
        }
    }
}