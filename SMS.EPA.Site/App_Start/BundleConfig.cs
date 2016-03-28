namespace SMS.EPA.Site
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Optimization;

    public static class BundleConfig
    {
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
                            "~/Libs/bootstrap.min.js",
                            "~/Libs/angular.js",
                            "~/Libs/angular-resource.js",
                            "~/Libs/angular-route.js",
                            "~/Libs/angular-translate.js",
                            "~/Libs/angular-filter/dist/angular-filter.js",
                            "~/Libs/ui-bootstrap-tpls-0.13.0.min.js",
                            "~/Libs/toastr.js",
                            "~/Libs/angular-multi-select.js",
                            "~/Libs/jquery-ui.min.js",
                            "~/Libs/datepicker.js",
                            "~/Libs/tai-placeholder.js",
                            "~/Libs/dirPagination.js",
                            "~/Libs/chosen.jquery.min.js",
                            "~/Libs/angular-chosen.js",
                            "~/Libs/ui-bootstrap-contextMenu.js",
                            "~/Libs/numeric-directive.js",
                            "~/Libs/spin.min.js",
                            "~/Libs/angular-spinner.min.js",
                            "~/Libs/floatThead/jquery.floatThead.min.js"));

            BuildBundleForFanexGrid(bundles);
            BuildBundleForStarRating(bundles);

            bundles.AddAndInclude(
                            "~/bundles/less",
                            "~/Content/Less/bootstrap.less",
                            "~/Content/Site/layout.less",
                            "~/SourceApp/Components/KPI/kpi.less",
                            "~/SourceApp/Components/Employee/employee.less",
                            "~/SourceApp/Components/Role/role.less",
                            "~/SourceApp/Components/TargetTemplate/targetTemplate.less",
                            "~/SourceApp/Components/FlowManagement/flowManagement.less",
                            "~/SourceApp/Components/TargetEmployeeSetting/targetEmployeeSetting.less",
                            "~/SourceApp/Components/Period/period.less",
                            "~/SourceApp/Components/EPA/epa.less",
                            "~/SourceApp/Components/Dashboard/dashboard.less",
                            "~/SourceApp/Components/AttendanceSummary/attendanceSummary.less",
                            "~/SourceApp/Components/Report/report.less");

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                            "~/SourceApp/app.js",
                            "~/SourceApp/routes.js",
                            "~/SourceApp/Components/Resources/resources-factory.js",
                            "~/SourceApp/Components/Resources/resources-controller.js",
                            "~/SourceApp/Components/Common/common-factory.js",
                            "~/SourceApp/Components/Common/common-model.js",
                            "~/SourceApp/Components/Common/common-controller.js",
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
                            "~/SourceApp/Components/FlowManagement/resultSettingFlow-controller.js",
                            "~/SourceApp/Components/TargetEmployeeSetting/targetEmployeeSetting-factory.js",
                            "~/SourceApp/Components/TargetEmployeeSetting/targetEmployeeSetting-model.js",
                            "~/SourceApp/Components/TargetEmployeeSetting/targetEmployeeSetting-controller.js",
                            "~/SourceApp/Components/Department/department-factory.js",
                            "~/SourceApp/Components/Department/department-model.js",
                            "~/SourceApp/Components/Department/department-controller.js",
                            "~/SourceApp/Components/Department/department-directive.js",
                            "~/SourceApp/Components/EPA/employeeEPA-directive.js",
                            "~/SourceApp/Components/EPA/employeeEPA-factory.js",
                            "~/SourceApp/Components/EPA/employeeEPA-model.js",
                            "~/SourceApp/Components/EPA/selfEvaluator-model.js",
                            "~/SourceApp/Components/EPA/reviewerManagement-model.js",
                            "~/SourceApp/Components/EPA/employeeEPA-controller.js",
                            "~/SourceApp/Components/EPA/MyEPA/myepa-controller.js",
                            "~/SourceApp/Components/EPA/Evaluator/evaluator-controller.js",
                            "~/SourceApp/Components/EPA/Reviewer/reviewer-controller.js",
                            "~/SourceApp/Components/EPA/Management/management-controller.js",
                            "~/SourceApp/Components/EPA/EPAByKPI/EPAByKPI-factory.js",
                            "~/SourceApp/Components/EPA/EPAByKPI/EPAByKPI-model.js",
                            "~/SourceApp/Components/EPA/EPAByKPI/EPAByKPI-controller.js",
                            "~/SourceApp/Components/Period/period-factory.js",
                            "~/SourceApp/Components/Period/period-model.js",
                            "~/SourceApp/Components/Period/period-directive.js",
                            "~/SourceApp/Components/Period/period-controller.js",
                            "~/SourceApp/Components/Period/period-employeeSetting-controller.js",
                            "~/SourceApp/Components/Dashboard/dashboard-factory.js",
                            "~/SourceApp/Components/Dashboard/dashboard-model.js",
                            "~/SourceApp/Components/Dashboard/dashboard-controller.js",
                            "~/SourceApp/Components/AttendanceSummary/attendanceSummary-factory.js",
                            "~/SourceApp/Components/AttendanceSummary/attendanceSummary-model.js",
                            "~/SourceApp/Components/AttendanceSummary/attendanceSummary-controller.js",
                             "~/SourceApp/Components/AttendanceSummary/attendanceSummary-directive.js",
                            "~/SourceApp/Components/EPA/RatingByKPI/ratingByKPI-factory.js",
                            "~/SourceApp/Components/EPA/RatingByKPI/ratingByKPI-model.js",
                            "~/SourceApp/Components/EPA/RatingByKPI/ratingByKPI-controller.js",
                            "~/SourceApp/Components/EPA/Convert/convertEPA-controller.js",
                            "~/SourceApp/Components/EPA/ResetScore/resetScore-controller.js",
                            "~/SourceApp/Components/Report/report-factory.js",
                            "~/SourceApp/Components/Report/report-model.js",
                            "~/SourceApp/Components/Report/report-directive.js",
                            "~/SourceApp/Components/Report/epaSummaryReport-controller.js",
                            "~/SourceApp/Components/Report/epaDetailReport-controller.js",
                            "~/SourceApp/Components/EPAHistory/epaHistory-factory.js",
                            "~/SourceApp/Components/EPAHistory/epaHistory-model.js",
                            "~/SourceApp/Components/EPAHistory/epaHistory-controller.js",
                            "~/SourceApp/Components/Error/error-controller.js",
                            "~/SourceApp/Components/MonthlyPeriod/monthlyPeriod-factory.js",
                            "~/SourceApp/Components/MonthlyPeriod/monthlyPeriod-model.js",
                            "~/SourceApp/Components/MonthlyPeriod/monthlyPeriod-directive.js",
                            "~/SourceApp/Components/MonthlyPeriod/monthlyPeriod-controller.js",
                            "~/SourceApp/Components/MonthlyPeriod/monthlyPeriod-employeeSetting-controller.js",
                            "~/SourceApp/Components/EPA/Evaluator/evaluatorViewState-service.js",
                            "~/SourceApp/Components/EPA/Reviewer/reviewerViewState-service.js",
                            "~/SourceApp/Components/EPA/Management/managementViewState-service.js",
                            "~/SourceApp/Components/EPAHistory/epaHistoryViewState-service.js",
                            "~/SourceApp/Components/KPIGroup/kpiGroupViewState-service.js",
                            "~/SourceApp/Components/MonthlyPeriod/monthlyPeriodViewState-service.js",
                            "~/SourceApp/Components/Period/periodViewState-service.js"));

            bundles.Add(new ScriptBundle("~/bundles/common/js").Include(
                            "~/SourceApp/Shared/epa-directives.js",
                            "~/SourceApp/Shared/epa-filter.js",
                            "~/SourceApp/Shared/error-factory.js",
                            "~/SourceApp/Shared/modalconfirm-controller.js",
                            "~/SourceApp/Shared/notification-factory.js",
                            "~/SourceApp/Shared/utilities-factory.js",
                            "~/SourceApp/Shared/enumerations-factory.js"));
        }

        public static void BuildBundleForFanexGrid(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/fanexgridcss")
                .Include("~/Libs/FanexGrid/Fanex.Grid.css", new CssRewriteUrlTransform())
                .Include("~/Libs/FanexGrid/Fanex.Pagination.css", new CssRewriteUrlTransform()));

            bundles.Add(new ScriptBundle("~/bundles/fanexgridjs").Include(
                "~/Libs/FanexGrid/Fanex.Grid.js",
                "~/Libs/FanexGrid/moment.min.js",
                "~/Libs/FanexGrid/numeral.min.js",
                "~/Libs/FanexGrid/Fanex.Pagination.js"));
        }

        public static void BuildBundleForStarRating(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/starratingcss")
                .Include("~/Libs/BootstrapStarRating/css/star-rating.min.css", new CssRewriteUrlTransform()));

            bundles.Add(new ScriptBundle("~/bundles/starratingjs")
                .Include("~/Libs/BootstrapStarRating/js/star-rating.js"));
        }
    }
}