'use strict';
/*global targetEmployeeSettingModule*/

targetEmployeeSettingModule.factory('targetEmployeeSettingFactory',
['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getTargetEmployeeSetting: function (groupId, employeeId, targetTypeId) {
            return $http.get(baseUrl + 'api/TargetEmployeeSetting/GetTargetEmployeeSetting',
                { params: { groupId: groupId, employeeId: employeeId, targetTypeId: targetTypeId } });
        },
        assignTargetTemplateToEmployee: function (targetEmployeeSetting) {
            return $http.put(baseUrl + 'api/TargetEmployeeSetting/AssignTargetTemplateToEmployee', targetEmployeeSetting);
        }
    };
}]);