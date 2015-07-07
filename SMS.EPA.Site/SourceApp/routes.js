'use strict';
/*exported employeeModule, commonModule*/

var employeeModule = angular.module("employeeModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/Employee/Index',
    { templateUrl: '/SourceApp/Components/Employee/Templates/index.html' });
} ]);

var commonModule = angular.module("commonModule", ['ngRoute']);