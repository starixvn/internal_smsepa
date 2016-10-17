(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('employeeModule').factory('employeeFactory', employeeFactory);
    employeeFactory.$inject = ['$http', 'baseUrl'];

    function employeeFactory($http, baseUrl) {
        return {
            assignRoleToEmployee: assignRoleToEmployee,
            changePassword: changePassword,
            createSpecialEmployee: createSpecialEmployee,
            getAllEmployee: getAllEmployee,
            getDefaultPassword: getDefaultPassword,
            getUserPermissionOnEmployee: getUserPermissionOnEmployee,
            resetPassword: resetPassword,
            updateHasMonthlyEPA: updateHasMonthlyEPA,
            updateSpecialEmployee: updateSpecialEmployee
        };

        function assignRoleToEmployee(employeeRoles) {
            return $http.put(baseUrl + 'api/Employee/AssignRoleToEmployee', employeeRoles);
        }

        function changePassword(employee) {
            return $http.put(baseUrl + 'api/Employee/ChangePassword', employee);
        }

        function createSpecialEmployee(employee) {
            return $http.post(baseUrl + 'api/Employee/CreateSpecialEmployee', employee);
        }

        function getDefaultPassword() {
            return $http.get(baseUrl + 'api/Employee/GetDefaultPassword');
        }

        function getAllEmployee(query, groupId, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA) {
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
        }

        function getUserPermissionOnEmployee() {
            return $http.get(baseUrl + 'api/Employee/GetUserPermissionOnEmployee');
        }

        function resetPassword(employeeId) {
            return $http.get(baseUrl + 'api/Employee/ResetPassword', { params: { employeeId: employeeId } });
        }

        function updateHasMonthlyEPA(employee) {
            return $http.put(baseUrl + 'api/Employee/UpdateHasMonthlyEPA', employee);
        }

        function updateSpecialEmployee(employee) {
            return $http.put(baseUrl + 'api/Employee/UpdateSpecialEmployee', employee);
        }
    }
})(angular);