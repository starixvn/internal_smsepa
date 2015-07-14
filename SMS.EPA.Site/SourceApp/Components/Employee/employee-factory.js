'use strict';
/*global employeeModule*/

employeeModule.factory('employeeFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getAllEmployee: function (query, groupId, isSpecialUser) {
            return $http.get(baseUrl + 'api/Employee/GetAllEmployee',
                { params: { query: query, groupId: groupId, isSpecialUser: isSpecialUser} });
        },
        getAllGroup: function () {
            return $http.get(baseUrl + 'api/Employee/GetAllGroup');
        },
        insertSpecialEmployee: function (employee) {
            return $http.post(baseUrl + 'api/Employee/InsertSpecialEmployee', employee);
        },
        updateSpecialEmployee: function (employee) {
            return $http.put(baseUrl + 'api/Employee/UpdateSpecialEmployee', employee);
        },
        getAllJobTitle: function () {
            return $http.get(baseUrl + 'api/Employee/GetAllJobTitle');
        },
        changePassword: function (employee) {
            return $http.put(baseUrl + 'api/Employee/ChangePassword', employee);
        },
        assignRoleToEmployee: function (employeeRoles) {
            return $http.put(baseUrl + 'api/Employee/AssignRoleToEmployee', employeeRoles);
        }
    };
} ]);