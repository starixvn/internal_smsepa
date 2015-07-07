// ----------------------------------------------------------------------
// <copyright file="SessionManager.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 10:32:44 AM">Created.</change>
// </history>

namespace SMS.EPA.Web
{
    using System.Web;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.Authentication;

    /// <summary>
    /// The session manager.
    /// </summary>
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

        /// <summary>
        /// Function to clear session manager.
        /// </summary>
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
    }
}
