(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetEmployeeSettingModule').factory('targetEmployeeSettingFactory', targetEmployeeSettingFactory);
    targetEmployeeSettingFactory.$inject = ['$http', 'baseUrl'];

    function targetEmployeeSettingFactory($http, baseUrl) {
        return {
            assignTargetTemplateToDepartment,
            assignTargetTemplateToEmployee,
            deleteTargetDepartmentSetting,
            deleteTargetEmployeeSetting,
            getTargetDepartmentSetting,
            getTargetEmployeeSetting,
            getUserPermissionOnTargetEmployeeSetting
        };

        function assignTargetTemplateToDepartment(params) {
            return $http.put(baseUrl + 'api/TargetEmployeeSetting/AssignTargetTemplateToDepartment', params);
        }

        function assignTargetTemplateToEmployee(params) {
            return $http.put(baseUrl + 'api/TargetEmployeeSetting/AssignTargetTemplateToEmployee', params);
        }

        function getTargetDepartmentSetting(departmentId, isGroup, targetTypeId, showAll) {
            return $http.get(baseUrl + 'api/TargetEmployeeSetting/GetTargetDepartmentSetting', {
                params: { departmentId, isGroup, targetTypeId, showAll }
            });
        }

        function getTargetEmployeeSetting(teamIds, employeeId, targetTypeId, showAll) {
            return $http.get(baseUrl + 'api/TargetEmployeeSetting/GetTargetEmployeeSetting', {
                params: { teamIds, employeeId, targetTypeId, showAll }
            });
        }

        function deleteTargetDepartmentSetting(targetTemplateId, departmentId, isGroup) {
            return $http['delete'](baseUrl + 'api/TargetEmployeeSetting/DeleteTargetDepartmentSetting', {
                params: { targetTemplateId, departmentId, isGroup }
            });
        }

        function deleteTargetEmployeeSetting(targetTemplateId, employeeId) {
            return $http['delete'](baseUrl + 'api/TargetEmployeeSetting/DeleteTargetEmployeeSetting', {
                params: { employeeId, targetTemplateId }
            });
        }

        function getUserPermissionOnTargetEmployeeSetting() {
            return $http.get(baseUrl + 'api/TargetEmployeeSetting/GetUserPermissionOnTargetEmployeeSetting');
        }
    }
})(angular);