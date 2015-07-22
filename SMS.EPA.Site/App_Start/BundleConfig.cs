namespace SMS.EPA.Site
{
    using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
                            "~/Libs/dirPagination.js",
                            "~/Libs/chosen.jquery.min.js",
                            "~/Libs/angular-chosen.js",
                            "~/Libs/moment.min.js"));

            bundles.AddAndInclude(
                            "~/bundles/less",
                            "~/Content/Less/bootstrap.less",
                            "~/Content/Site/layout.less",
                            "~/SourceApp/Components/KPI/kpi.less",
                            "~/SourceApp/Components/Employee/employee.less",
                            "~/SourceApp/Components/Role/role.less",
                            "~/SourceApp/Components/TargetTemplate/targetTemplate.less",
                            "~/SourceApp/Components/FlowManagement/flowManagement.less",
                            "~/SourceApp/Components/TargetEmployeeSetting/targetEmployeeSetting.less");

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                            "~/SourceApp/app.js",
                            "~/SourceApp/routes.js",
                            "~/SourceApp/Components/Resources/resources-factory.js",
                            "~/SourceApp/Components/Resources/resources-controller.js",
                            "~/SourceApp/Components/Common/common-factory.js",
                            "~/SourceApp/Components/Common/common-model.js",
                            "~/SourceApp/Components/Employee/employee-factory.js",
                            "~/SourceApp/Components/Employee/employee-model.js",
                            "~/SourceApp/Components/Employee/employee-controller.js",
                            "~/SourceApp/Components/Employee/employee-directive.js",
                            "~/SourceApp/Components/KPI/kpi-factory.js",
                            "~/SourceApp/Components/KPI/kpi-model.js",
                            "~/SourceApp/Components/KPI/kpi-controller.js",
                            "~/SourceApp/Components/KPIGroup/kpiGroup-factory.js",
                            "~/SourceApp/Components/KPIGroup/kpiGroup-model.js",
                            "~/SourceApp/Components/KPIGroup/kpiGroup-controller.js",
                            "~/SourceApp/Components/Role/role-directive.js",
                            "~/SourceApp/Components/Role/role-factory.js",
                            "~/SourceApp/Components/Role/role-model.js",
                            "~/SourceApp/Components/Role/role-controller.js",
                            "~/SourceApp/Components/TargetTemplate/targetTemplate-factory.js",
                            "~/SourceApp/Components/TargetTemplate/targetTemplate-model.js",
                            "~/SourceApp/Components/TargetTemplate/targetTemplate-directive.js",
                            "~/SourceApp/Components/TargetTemplate/targetTemplate-controller.js",
                            "~/SourceApp/Components/FlowManagement/flowManagement-factory.js",
                            "~/SourceApp/Components/FlowManagement/flowManagement-model.js",
                            "~/SourceApp/Components/FlowManagement/flowManagement-controller.js",
                            "~/SourceApp/Components/FlowManagement/departmentFlow-controller.js",
                            "~/SourceApp/Components/FlowManagement/personalFlow-controller.js",
                            "~/SourceApp/Components/TargetEmployeeSetting/targetEmployeeSetting-factory.js",
                            "~/SourceApp/Components/TargetEmployeeSetting/targetEmployeeSetting-model.js",
                            "~/SourceApp/Components/TargetEmployeeSetting/targetEmployeeSetting-controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/common/js").Include(
                            "~/SourceApp/Shared/epa-directives.js",
                            "~/SourceApp/Shared/error-factory.js",
                            "~/SourceApp/Shared/modalconfirm-controller.js",
                            "~/SourceApp/Shared/notification-factory.js",
                            "~/SourceApp/Shared/utilities-factory.js"));
        }
    }
}