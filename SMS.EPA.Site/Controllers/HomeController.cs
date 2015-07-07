// ----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/2/2015 2:57:54 PM">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers
{
    using System.Web.Mvc;
    using SMS.EPA.Web;

    /// <summary>
    /// The Home controller.
    /// </summary>
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (SessionManager.CurrentUser != null)
            {
                return View();
            }

            return RedirectToAction("Login", "Auth");
        }
    }
}
