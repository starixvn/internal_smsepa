(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('dashboardModule').factory('dashboardFactory', dashboardFactory);
    dashboardFactory.$inject = ['$http', 'baseUrl'];

    function dashboardFactory($http, baseUrl) {
        return {
            getDashboard,
            getToDoList,
            getJLSDashboard,
            getAnnouncement
        };

        function getDashboard() {
            return $http.get(baseUrl + 'api/Dashboard/GetDashboard');
        }

        function getToDoList() {
            return $http.get(baseUrl + 'api/Dashboard/GetToDoList');
        }

        function getJLSDashboard() {
            return $http.get(baseUrl + 'api/Dashboard/GetJobLevelStandardDashboard');
        }

        function getAnnouncement() {
            return $http.get(baseUrl + 'api/Dashboard/GetAnnouncement');
        }
    }
})(angular);