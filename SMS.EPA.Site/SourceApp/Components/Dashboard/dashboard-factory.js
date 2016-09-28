(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('dashboardModule').factory('dashboardFactory', dashboardFactory);
    dashboardFactory.$inject = ['$http', 'baseUrl'];

    function dashboardFactory($http, baseUrl) {
        return {
            getDashboard: getDashboard,
            getToDoList: getToDoList
        };

        function getDashboard() {
            return $http.get(baseUrl + 'api/Dashboard/GetDashboard');
        }

        function getToDoList() {
            return $http.get(baseUrl + 'api/Dashboard/GetToDoList');
        }
    }
})(angular);