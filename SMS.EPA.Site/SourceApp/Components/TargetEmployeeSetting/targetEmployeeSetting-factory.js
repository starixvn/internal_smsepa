(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetEmployeeSettingModule').factory('targetEmployeeSettingFactory', targetEmployeeSettingFactory);
    targetEmployeeSettingFactory.$inject = ['$http', 'baseUrl'];

    function targetEmployeeSettingFactory($http, baseUrl) {
        return {
            assignTargetTemplateToDepartment: assignTargetTemplateToDepartment,
            assignTargetTemplateToEmployee: assignTargetTemplateToEmployee,
            deleteTargetDepartmentSetting: deleteTargetDepartmentSetting,
            deleteTargetEmployeeSetting: deleteTargetEmployeeSetting,
            getTargetDepartmentSetting: getTargetDepartmentSetting,
            getTargetEmployeeSetting: getTargetEmployeeSetting,
            getUserPermissionOnTargetEmployeeSetting: getUserPermissionOnTargetEmployeeSetting
        };

        function assignTargetTemplateToDepartment(targetSetting) {
            return $http.put(baseUrl + 'api/TargetEmployeeSetting/AssignTargetTemplateToDepartment', targetSetting);
        }

        function assignTargetTemplateToEmployee(targetEmployeeSetting) {
            return $http.put(baseUrl + 'api/TargetEmployeeSetting/AssignTargetTemplateToEmployee', targetEmployeeSetting);
        }

        function getTargetDepartmentSetting(departmentId, isGroup, targetTypeId, showAll) {
            return $http.get(baseUrl + 'api/TargetEmployeeSetting/GetTargetDepartmentSetting', {
                params: {
                    departmentId: departmentId,
                    isGroup: isGroup,
                    targetTypeId: targetTypeId,
                    showAll: showAll
                }
            });
        }

        function getTargetEmployeeSetting(groupId, employeeId, targetTypeId, showAll) {
            return $http.get(baseUrl + 'api/TargetEmployeeSetting/GetTargetEmployeeSetting', {
                params: {
                    groupId: groupId,
                    employeeId: employeeId,
                    targetTypeId: targetTypeId,
                    showAll: showAll
                }
            });
        }

        function deleteTargetDepartmentSetting(targetTemplateId, departmentId, isGroup) {
            return $http['delete'](baseUrl + 'api/TargetEmployeeSetting/DeleteTargetDepartmentSetting', {
                params: {
                    targetTemplateId: targetTemplateId,
                    departmentId: departmentId,
                    isGroup: isGroup
                }
            });
        }

        function deleteTargetEmployeeSetting(targetTemplateId, employeeId) {
            return $http['delete'](baseUrl + 'api/TargetEmployeeSetting/DeleteTargetEmployeeSetting', {
                params: {
                    employeeId: employeeId,
                    targetTemplateId: targetTemplateId
                }
            });
        }

        function getUserPermissionOnTargetEmployeeSetting() {
            return $http.get(baseUrl + 'api/TargetEmployeeSetting/GetUserPermissionOnTargetEmployeeSetting');
        }
    }
})(angular);