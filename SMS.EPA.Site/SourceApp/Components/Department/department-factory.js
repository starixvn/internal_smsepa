(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('departmentModule').factory('departmentFactory', departmentFactory);
    departmentFactory.$inject = ['$http', 'baseUrl'];

    function departmentFactory($http, baseUrl) {
        return {
            getDepartment: getDepartment
        };

        function getDepartment(parentId, isActive) {
            return $http.get(baseUrl + 'api/Department/GetDepartment', { params: { parentId: parentId, isActive: isActive } });
        }
    }
})(angular);