(function (angular) {
    'use strict';

    angular.module('dashboardModule').factory('dashboardFactory', dashboardFactory);
    dashboardFactory.$inject = ['$http', 'baseUrl'];

    function dashboardFactory($http, baseUrl) {
        return {
            getDashboard: getDashboard
        };

        function getDashboard() {
            return $http.get(baseUrl + 'api/Dashboard/GetDashboard');
        }
    }
})(angular);