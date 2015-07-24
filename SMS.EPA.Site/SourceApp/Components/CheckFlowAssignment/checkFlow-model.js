'use strict';
/*global checkFlowAssignmentModule*/

checkFlowAssignmentModule.factory('CheckFlowModel',
['checkFlowFactory', 'errorFactory', 'notificationFactory',
function (checkFlowFactory, errorFactory, notificationFactory) {
    var checkFlowModel = {};
    checkFlowModel.employeeSettings = [];

    checkFlowModel.getEmployeeFlowTargetSetting = function (groupId, employeeId, targetTypeId) {
        return checkFlowFactory.getEmployeeFlowTargetSetting(groupId, employeeId, targetTypeId).success(function (employeeSettings) {
            checkFlowModel.employeeSettings = employeeSettings;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    checkFlowModel.updateEmployeeFlowTargetSetting = function (employeeAssignments) {
        return checkFlowModel.updateEmployeeFlowTargetSetting(employeeAssignments).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdateSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return checkFlowModel;
}]);