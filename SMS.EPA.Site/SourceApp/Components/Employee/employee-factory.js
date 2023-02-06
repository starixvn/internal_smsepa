(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('employeeModule').factory('employeeFactory', employeeFactory);
    employeeFactory.$inject = ['$http', 'baseUrl'];

    function employeeFactory($http, baseUrl) {
        return {
            assignRoleToEmployee,
            changePassword,
            createSpecialEmployee,
            getEmployee,
            getDefaultPassword,
            getUserPermissionOnEmployee,
            resetPassword,
            updateHasMonthlyEPA,
            updateSpecialEmployee,
            updateJobLevel
        };

        function getEmployee(query, teamIds, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA, jobLevelId) {
            return $http.get(baseUrl + 'api/Employee/GetAllEmployee',
                {
                    params: { query, teamIds, isLocal, isSpecialUser, isDisabledUser, hasMonthlyEPA, jobLevelId }
                });
        }

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

        function updateJobLevel(employee) {
            return $http.put(baseUrl + 'api/Employee/UpdateJobLevel', employee);
        }
    }
})(angular);