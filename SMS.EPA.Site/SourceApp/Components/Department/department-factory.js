'use strict';
/*global departmentModule*/

departmentModule.factory('departmentFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getDepartment: function (parentId, isActive) {
            return $http.get(baseUrl + 'api/Department/GetDepartment', { params: { parentId: parentId, isActive: isActive } });
        }
    };
}]);