'use strict';
/*global checkFlowAssignmentModule*/

checkFlowAssignmentModule.factory('checkFlowFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEmployeeFlowTargetSetting: function (groupId, employeeId, targetTypeId)
        {
            return $http.get(baseUrl + 'api/CheckFlowAssignment/GetEmployeeFlowTargetSetting', 
                    { params: { groupId: groupId, employeeId: employeeId, targetTypeId: targetTypeId }});
        },
        updateEmployeeFlowTargetSetting: function (employeeAssignments)
        {
            return $http.put(baseUrl + 'api/CheckFlowAssignment/UpdateEmployeeFlowTargetSetting', employeeAssignments);
        }
    };
}]);