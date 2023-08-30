(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('departmentModule').factory('departmentFactory', departmentFactory);
    departmentFactory.$inject = ['$http', 'baseUrl'];

    function departmentFactory($http, baseUrl) {
        return {
            getDepartment: getDepartment
        };

        function getDepartment() {
            return $http.get(baseUrl + 'api/Department/GetDepartment');
        }
    }
})(angular);