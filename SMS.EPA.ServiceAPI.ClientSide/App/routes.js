(function (angular) {
    'use strict';

    angular.module('app').config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/token/index', {
            templateUrl: '/App/token/index.html',
            controller: 'tokenController',
        }).
        when('/department/index', {
            templateUrl: '/App/department/index.html',
            controller: 'departmentController'
        }).
        otherwise({
            redirectTo: '/department/index'
        });
    }]);
})(angular);