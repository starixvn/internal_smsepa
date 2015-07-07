// ----------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/2/2015 10:17:49 AM">Created.</change>
// </history>

namespace SMS.EPA.Site
{
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// The filter config.
    /// </summary>
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}