'use strict';
/*exported employeeModule, commonModule, kpiModule, kpiGroupModule, roleModule, targetTemplateModule*/
/*exported flowMngModule, targetEmployeeSettingModule, departmentModule*/
/*exported epaModule, periodModule, dashboardModule, attendanceModule, reportModule, epaHistoryModule, errorModule*/
/*exported monthlyPeriodModule*/

var employeeModule = angular.module("employeeModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Employee/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/Employee/Templates/index.html' });
}]);

var commonModule = angular.module("commonModule", ['ngRoute']);

var kpiModule = angular.module("kpiModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/KPI/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/KPI/Templates/index.html' })
    .otherwise({redirectTo: '/error'});
}]);

var errorModule = angular.module('errorModule', ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/error',
    { templateUrl: baseUrl + 'SourceApp/Components/Error/Templates/error.html' });
}]);

var kpiGroupModule = angular.module("kpiGroupModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/KPIGroup/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/KPIGroup/Templates/index.html' });
}]);

var roleModule = angular.module("roleModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Role/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/Role/Templates/index.html' });
}]);

var targetTemplateModule = angular.module("targetTemplateModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/TargetTemplate/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/TargetTemplate/Templates/index.html' });
    $routeProvider.when('/TargetTemplate/Create',
    { templateUrl: baseUrl + 'SourceApp/Components/TargetTemplate/Templates/create.html' });
    $routeProvider.when('/TargetTemplate/Update/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/TargetTemplate/Templates/update.html' });
    $routeProvider.when('/TargetTemplate/Detail/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/TargetTemplate/Templates/detail.html' });
}]);

var flowMngModule = angular.module("flowMngModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Flow/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/FlowManagement/Templates/index.html' });
    $routeProvider.when('/Flow/Create',
    { templateUrl: baseUrl + 'SourceApp/Components/FlowManagement/Templates/create.html' });
    $routeProvider.when('/Flow/Update/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/FlowManagement/Templates/update.html' });
}]);

var targetEmployeeSettingModule = angular.module("targetEmployeeSettingModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/TargetEmployeeSetting/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/TargetEmployeeSetting/Templates/index.html' });
}]);

var departmentModule = angular.module("departmentModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Department/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/Department/Templates/index.html' });
}]);

var epaModule = angular.module("epaModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/SelfEvaluation/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/MyEPA/Templates/index.html' });
    $routeProvider.when('/SelfEvaluation/Detail/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/MyEPA/Templates/detail.html' });
    $routeProvider.when('/Evaluator/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/Evaluator/Templates/index.html' });
    $routeProvider.when('/Evaluator/Detail/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/Evaluator/Templates/detail.html' });
    $routeProvider.when('/Reviewer/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/Reviewer/Templates/index.html' });
    $routeProvider.when('/Reviewer/Detail/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/Reviewer/Templates/detail.html' });
    $routeProvider.when('/Management/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/Management/Templates/index.html' });
    $routeProvider.when('/Management/Detail/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/Management/Templates/detail.html' });
    $routeProvider.when('/EPA/Detail/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/Templates/viewDetail.html' });
    $routeProvider.when('/RatingByKPI/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/RatingByKPI/Templates/index.html' });
    $routeProvider.when('/OldEPA/Convert',
    { templateUrl: baseUrl + 'SourceApp/Components/EPA/Convert/Templates/detail.html' });
}]);

var periodModule = angular.module("periodModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Period/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/Period/Templates/index.html' });
    $routeProvider.when('/Period/Update/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/Period/Templates/edit.html' });
}]);

var dashboardModule = angular.module("dashboardModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/',
    { templateUrl: baseUrl + 'SourceApp/Components/Dashboard/Templates/index.html' });
}]);

var attendanceModule = angular.module("attendanceModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Discipline/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/AttendanceSummary/Templates/index.html' });
    $routeProvider.when('/Discipline/Detail/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/AttendanceSummary/Templates/detail.html' });
    $routeProvider.when('/Discipline/Detail',
    { templateUrl: baseUrl + 'SourceApp/Components/AttendanceSummary/Templates/detail.html' });
}]);

var reportModule = angular.module("reportModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Report/EPASummaryReport',
    { templateUrl: baseUrl + 'SourceApp/Components/Report/Templates/EPASummaryReport.html' });
    $routeProvider.when('/Report/EPADetailReport',
    { templateUrl: baseUrl + 'SourceApp/Components/Report/Templates/EPADetailReport.html' });
}]);

var epaHistoryModule = angular.module("epaHistoryModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/EPAHistory/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/EPAHistory/Templates/index.html' });
}]);

var monthlyPeriodModule = angular.module("monthlyPeriodModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/MonthlyPeriod/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/MonthlyPeriod/Templates/index.html' });
    $routeProvider.when('/MonthlyPeriod/Update/:id',
    { templateUrl: baseUrl + 'SourceApp/Components/MonthlyPeriod/Templates/edit.html' });
}]);