(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').factory('jobDescriptionFactory', jobDescriptionFactory);
    jobDescriptionFactory.$inject = ['$http', 'baseUrl'];

    function jobDescriptionFactory($http, baseUrl) {
        return {
            getUserPermission: getUserPermission
        };

        function getUserPermission() {
            return $http.get(baseUrl + 'api/JobDescription/GetUserPermission');
        }
    }
})(angular);