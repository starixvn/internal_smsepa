'use strict';
/*global employeeModule*/

employeeModule.factory('employeeFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getAllEmployee: function (query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA) {
            return $http.get(baseUrl + 'api/Employee/GetAllEmployee',
                {
                    params: {
                        query: query,
                        groupId: groupId,
                        isLocal: isLocal,
                        isSpecialUser: isSpecialUser,
                        isDisabledUser: isDisabledUser,
                        hasMonthlyEPA: hasMonthlyEPA
                    }
                });
        },
        getAllGroup: function () {
            return $http.get(baseUrl + 'api/Employee/GetAllGroup');
        },
        createSpecialEmployee: function (employee) {
            return $http.post(baseUrl + 'api/Employee/CreateSpecialEmployee', employee);
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
        },
        getDefaultPassword: function () {
            return $http.get(baseUrl + 'api/Employee/GetDefaultPassword');
        },
        resetPassword: function (employeeId) {
            return $http.get(baseUrl + 'api/Employee/ResetPassword', { params: { employeeId: employeeId } });
        },
        updateHasMonthlyEPA: function (employee) {
            return $http.put(baseUrl + 'api/Employee/UpdateHasMonthlyEPA', employee);
        },
        getUserPermissionOnEmployee: function () {
            return $http.get(baseUrl + 'api/Employee/GetUserPermissionOnEmployee');
        }
    };
} ]);