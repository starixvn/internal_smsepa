// ----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/7/2015 11:03:13 AM">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using System.Web.Security;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Web;

    /// <summary>
    /// The base controller.
    /// </summary>
    public class BaseController : ApiController
    {
        /// <summary>
        /// Gets Current Session.
        /// </summary>
        /// <value>Current Session.</value>
        protected UserSession CurrentUser
        {
            get
            {
                var currentUser = SessionManager.CurrentUser;
                if (currentUser == null)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                return currentUser;
            }
        }
    }
}
