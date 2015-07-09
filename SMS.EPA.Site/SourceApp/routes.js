'use strict';
/*exported employeeModule, commonModule, kpiModule, kpiGroupModule*/

var employeeModule = angular.module("employeeModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/Employee/Index',
    { templateUrl: '/SourceApp/Components/Employee/Templates/index.html' });
} ]);

var commonModule = angular.module("commonModule", ['ngRoute']);

var kpiModule = angular.module("kpiModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/KPI/Index',
    { templateUrl: '/SourceApp/Components/KPI/Templates/index.html' });
} ]);

var kpiGroupModule = angular.module("kpiGroupModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/KPIGroup/Index',
    { templateUrl: '/SourceApp/Components/KPIGroup/Templates/index.html' });
} ]);