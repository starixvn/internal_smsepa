'use strict';
/*global departmentModule*/

departmentModule.factory('departmentFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        createDepartment: function (department) {
            return $http.post(baseUrl + 'api/Department/CreateDepartment', department);
        },
        updateDepartment: function (department) {
            return $http.put(baseUrl + 'api/Department/UpdateDepartment', department);
        },
        deleteDepartment: function (departmentId) {
            return $http['delete'](baseUrl + 'api/Department/DeleteDepartment', { params: { departmentId: departmentId } });
        }
    };
}]);