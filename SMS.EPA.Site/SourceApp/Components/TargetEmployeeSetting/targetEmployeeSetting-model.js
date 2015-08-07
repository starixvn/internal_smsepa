'use strict';
/*global targetEmployeeSettingModule*/

targetEmployeeSettingModule.factory('TargetEmployeeSettingModel',
['targetEmployeeSettingFactory', 'notificationFactory', 'errorFactory',
function (targetEmployeeSettingFactory, notificationFactory, errorFactory) {
    var targetEmployeeSettingModel = {};

    targetEmployeeSettingModel.targetEmployeeSettings = [];
    targetEmployeeSettingModel.targetDepartmentSettings = [];

    targetEmployeeSettingModel.getTargetEmployeeSetting = function (groupId, employeeId, targetTypeId, showAll) {
        return targetEmployeeSettingFactory.getTargetEmployeeSetting(groupId, employeeId, targetTypeId, showAll)
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
            notificationFactory.error("SelectEmployeeBeforeAssignWarning");
        }
    };

    targetEmployeeSettingModel.openPopupDeleteTargetEmployeeSetting = function (targetTemplate, employeeId) {
        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return targetTemplate.Name;
                },
                action: function () {
                    return 'delete';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            return targetEmployeeSettingFactory.deleteTargetEmployeeSetting(targetTemplate.TargetTemplateId, employeeId)
                .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("DeletedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    targetEmployeeSettingModel.getTargetDepartmentSetting = function () {
        targetEmployeeSettingFactory.getTargetDepartmentSetting().success(function (targetDepartments) {
            targetEmployeeSettingModel.targetDepartmentSettings = targetDepartments;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    targetEmployeeSettingModel.assignTargetTemplateToDepartment = function (targetSetting) {
        if (targetSetting.DepartmentSettings) {
            targetEmployeeSettingFactory.assignTargetTemplateToDepartment(targetSetting).success(function (response) {
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
        }
        else {
            notificationFactory.error("SelectEmployeeBeforeAssignWarning");
        }
    };

    targetEmployeeSettingModel.openPopupDeleteTargetDepartmentSetting = function (targetTemplate, departmentId) {
        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return targetTemplate.Name;
                },
                action: function () {
                    return 'delete';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            return targetEmployeeSettingFactory.deleteTargetDepartmentSetting(targetTemplate.TargetTemplateId, departmentId, targetTemplate.IsGroup)
                .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("DeletedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    return targetEmployeeSettingModel;
}]);