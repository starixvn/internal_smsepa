'use strict';
/*global targetEmployeeSettingModule*/

targetEmployeeSettingModule.factory('TargetEmployeeSettingModel',
['targetEmployeeSettingFactory', 'notificationFactory', 'errorFactory',
function (targetEmployeeSettingFactory, notificationFactory, errorFactory) {
    var targetEmployeeSettingModel = {};

    targetEmployeeSettingModel.targetEmployeeSettings = [];

    targetEmployeeSettingModel.getTargetEmployeeSetting = function (groupId, employeeId, targetTypeId) {
        return targetEmployeeSettingFactory.getTargetEmployeeSetting(groupId, employeeId, targetTypeId)
        .success(function (targetEmployeeSettings) {
            targetEmployeeSettingModel.targetEmployeeSettings = targetEmployeeSettings;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    targetEmployeeSettingModel.assignTargetTemplateToEmployee = function (targetEmployeeSetting) {
        if (targetEmployeeSetting.employeeIds) {
            return targetEmployeeSettingFactory.assignTargetTemplateToEmployee(targetEmployeeSetting)
            .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdateSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
        else {
            notificationFactory.error("SelectEmployeeBeforeAssingWarning");
        }
    };

    return targetEmployeeSettingModel;
}]);