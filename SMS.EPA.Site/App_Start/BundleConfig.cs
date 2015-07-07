// ----------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/2/2015 10:00:37 AM">Created.</change>
// </history>

namespace SMS.EPA.Site
{
    using System.Web;
    using System.Web.Optimization;

    /// <summary>
    /// The bundle config.
    /// </summary>
    public static class BundleConfig
    {
        /// <summary>
        /// Register Bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.AddAndInclude(
                            "~/Content/login/css",
                            "~/Content/Less/bootstrap.less",
                            "~/Content/Site/auth/style.less");

            bundles.Add(new StyleBundle("~/Content/Common/css").Include(
                            "~/Content/Common/bootstrap-theme.min.css",
                            "~/Content/Common/toastr.css",
                            "~/Content/Common/angular-multi-select.css",
                            "~/Content/Common/jquery-ui.min.css",
                            "~/Content/Common/chosen.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/vendor").Include(
                            "~/Libs/jquery-2.1.0.min.js",
                            "~/Libs/angular.js",
                            "~/Libs/angular-resource.min.js",
                            "~/Libs/angular-route.min.js",
                            "~/Libs/angular-translate.js",
                            "~/Libs/ui-bootstrap-tpls-0.13.0.min.js",
                            "~/Libs/toastr.js",
                            "~/Libs/angular-multi-select.js",
                            "~/Libs/jquery-ui.min.js",
                            "~/Libs/datepicker.js",
                            "~/Libs/tai-placeholder.js",
                            "~/Libs/dirPagination.js"));

            bundles.AddAndInclude(
                            "~/bundles/less",
                            "~/Content/Less/bootstrap.less",
                            "~/Content/Site/layout.less");

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                            "~/SourceApp/app.js",
                            "~/SourceApp/routes.js",
                            "~/SourceApp/Components/Resources/resources-factory.js",
                            "~/SourceApp/Components/Resources/resources-controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/common/js").Include(
                            "~/SourceApp/Shared/epa-directives.js",
                            "~/SourceApp/Shared/error-factory.js",
                            "~/SourceApp/Shared/modalconfirm-controller.js",
                            "~/SourceApp/Shared/notification-factory.js",
                            "~/SourceApp/Shared/utilities-factory.js"));
        }
    }
}