'use strict';
/*exported employeeModule, commonModule, kpiModule, kpiGroupModule, roleModule, targetTemplateModule*/

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

var roleModule = angular.module("roleModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/Role/Index',
    { templateUrl: '/SourceApp/Components/Role/Templates/index.html' });
} ]);

var targetTemplateModule = angular.module("targetTemplateModule", ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/TargetTemplate/Index',
    { templateUrl: '/SourceApp/Components/TargetTemplate/Templates/index.html' });
    $routeProvider.when('/TargetTemplate/Create',
    { templateUrl: '/SourceApp/Components/TargetTemplate/Templates/create.html' });
    $routeProvider.when('/TargetTemplate/Update/:id',
    { templateUrl: '/SourceApp/Components/TargetTemplate/Templates/update.html' });
    $routeProvider.when('/TargetTemplate/Detail/:id',
    { templateUrl: '/SourceApp/Components/TargetTemplate/Templates/detail.html' });
} ]);