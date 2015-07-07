'use strict';
/*exported employeeModule, commonModule, kpiModule*/

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