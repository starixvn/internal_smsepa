'use strict';
/*global targetEmployeeSettingModule*/

targetEmployeeSettingModule.factory('TargetEmployeeSettingModel',
['$modal', 'targetEmployeeSettingFactory', 'notificationFactory', 'errorFactory',
function ($modal, targetEmployeeSettingFactory, notificationFactory, errorFactory) {
    var targetEmployeeSettingModel = {};

    targetEmployeeSettingModel.targetEmployeeSettings = [];
    targetEmployeeSettingModel.targetDepartmentSettings = [];
    targetEmployeeSettingModel.permission = null;

    targetEmployeeSettingModel.getPermissionOnTargetEmployeeSetting = function () {
        return targetEmployeeSettingFactory.getUserPermissionOnTargetEmployeeSetting().success(function (permission) {
            targetEmployeeSettingModel.permission = permission;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

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
                    notificationFactory.success("UpdatedSuccessfully");
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

    targetEmployeeSettingModel.openPopupDeleteTargetEmployeeSetting = function (targetTemplate, employee) {
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
            return targetEmployeeSettingFactory.deleteTargetEmployeeSetting(targetTemplate.TargetTemplateId, employee.Employee.EmployeeId)
                .success(function (response) {
                if (response.IsSuccessful) {
                    var index = employee.TargetTemplates.indexOf(targetTemplate);
                    employee.TargetTemplates.splice(index, 1);
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

    targetEmployeeSettingModel.getTargetDepartmentSetting = function (departmentId, isGroup, targetTypeId, showAll) {
        return targetEmployeeSettingFactory.getTargetDepartmentSetting(departmentId, isGroup, targetTypeId, showAll).success(function (targetDepartments) {
            targetEmployeeSettingModel.targetDepartmentSettings = targetDepartments;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    targetEmployeeSettingModel.assignTargetTemplateToDepartment = function (targetSetting) {
        if (targetSetting.departmentSettings.length > 0) {
            return targetEmployeeSettingFactory.assignTargetTemplateToDepartment(targetSetting).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
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
            notificationFactory.error("SelectDepartmentBeforeAssignWarning");
        }
    };

    targetEmployeeSettingModel.openPopupDeleteTargetDepartmentSetting = function (targetTemplate, department) {
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
            return targetEmployeeSettingFactory.deleteTargetDepartmentSetting(targetTemplate.TargetTemplateId, department.Department.DepartmentId, department.Department.IsGroup)
                .success(function (response) {
                if (response.IsSuccessful) {
                    var index = department.TargetTemplates.indexOf(targetTemplate);
                    department.TargetTemplates.splice(index, 1);
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