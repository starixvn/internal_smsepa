'use strict';
/*global targetEmployeeSettingModule*/

targetEmployeeSettingModule.factory('targetEmployeeSettingFactory',
['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getTargetEmployeeSetting: function (groupId, employeeId, targetTypeId, showAll) {
            return $http.get(baseUrl + 'api/TargetEmployeeSetting/GetTargetEmployeeSetting',
                { params: { groupId: groupId, employeeId: employeeId, targetTypeId: targetTypeId, showAll: showAll } });
        },
        assignTargetTemplateToEmployee: function (targetEmployeeSetting) {
            return $http.put(baseUrl + 'api/TargetEmployeeSetting/AssignTargetTemplateToEmployee', targetEmployeeSetting);
        },
        deleteTargetEmployeeSetting: function (targetTemplateId, employeeId) {
            return $http['delete'](baseUrl + 'api/TargetEmployeeSetting/DeleteTargetEmployeeSetting',
                { params: { employeeId: employeeId, targetTemplateId: targetTemplateId } });
        },
        getTargetDepartmentSetting: function (departmentId, isGroup, targetTypeId, showAll) {
            return $http.get(baseUrl + 'api/TargetEmployeeSetting/GetTargetDepartmentSetting', 
                { params: { departmentId: departmentId, isGroup: isGroup, targetTypeId: targetTypeId, showAll: showAll }});
        },
        assignTargetTemplateToDepartment: function (targetSetting) {
            return $http.put(baseUrl + 'api/TargetEmployeeSetting/AssignTargetTemplateToDepartment', targetSetting);
        },
        deleteTargetDepartmentSetting: function (targetTemplateId, departmentId, isGroup) {
            return $http['delete'](baseUrl + 'api/TargetEmployeeSetting/DeleteTargetDepartmentSetting', 
                { params: { targetTemplateId: targetTemplateId, departmentId: departmentId, isGroup: isGroup }});
        }
    };
}]);