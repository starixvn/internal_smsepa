'use strict';
/*exported employeeModule, commonModule, kpiModule, kpiGroupModule, roleModule, targetTemplateModule*/
/*exported flowMngModule, targetEmployeeSettingModule, checkFlowAssignmentModule*/

var employeeModule = angular.module("employeeModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Employee/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/Employee/Templates/index.html' });
} ]);

var commonModule = angular.module("commonModule", ['ngRoute']);

var kpiModule = angular.module("kpiModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/KPI/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/KPI/Templates/index.html' });
} ]);

var kpiGroupModule = angular.module("kpiGroupModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/KPIGroup/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/KPIGroup/Templates/index.html' });
} ]);

var roleModule = angular.module("roleModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Role/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/Role/Templates/index.html' });
} ]);

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
} ]);

var flowMngModule = angular.module("flowMngModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/Flow/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/FlowManagement/Templates/index.html' });
}]);

var targetEmployeeSettingModule = angular.module("targetEmployeeSettingModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/TargetEmployeeSetting/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/TargetEmployeeSetting/Templates/index.html' });
}]);

var checkFlowAssignmentModule = angular.module("checkFlowAssignmentModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    var baseUrl = $("base").first().attr("href");
    $routeProvider.when('/CheckAssignment/Index',
    { templateUrl: baseUrl + 'SourceApp/Components/CheckFlowAssignment/Templates/index.html' });
}]);