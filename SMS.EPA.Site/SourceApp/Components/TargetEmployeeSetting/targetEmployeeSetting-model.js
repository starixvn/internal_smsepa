(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetEmployeeSettingModule').factory('TargetEmployeeSettingModel', targetEmployeeSettingModel);
    targetEmployeeSettingModel.$inject = ['$uibModal', 'targetEmployeeSettingFactory', 'notificationFactory', 'errorFactory'];

    function targetEmployeeSettingModel($uibModal, targetEmployeeSettingFactory, notificationFactory, errorFactory) {
        var model = {
            permission: 0,
            targetDepartmentSettings: [],
            targetEmployeeSettings: [],
            assignTargetTemplateToDepartment: assignTargetTemplateToDepartment,
            assignTargetTemplateToEmployee: assignTargetTemplateToEmployee,
            getPermissionOnTargetEmployeeSetting: getPermissionOnTargetEmployeeSetting,
            getTargetDepartmentSetting: getTargetDepartmentSetting,
            getTargetEmployeeSetting: getTargetEmployeeSetting,
            openPopupDeleteTargetDepartmentSetting: openPopupDeleteTargetDepartmentSetting,
            openPopupDeleteTargetEmployeeSetting: openPopupDeleteTargetEmployeeSetting
        };

        return model;

        function assignTargetTemplateToDepartment(targetSetting) {
            if (targetSetting.departmentSettings.length > 0) {
                return targetEmployeeSettingFactory.assignTargetTemplateToDepartment(targetSetting).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success('UpdatedSuccessfully');
                    } else {
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            } else {
                notificationFactory.error('SelectDepartmentBeforeAssignWarning');
            }
        }

        function assignTargetTemplateToEmployee(targetEmployeeSetting) {
            if (targetEmployeeSetting.employeeIds) {
                return targetEmployeeSettingFactory.assignTargetTemplateToEmployee(targetEmployeeSetting).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success('UpdatedSuccessfully');
                    } else {
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            } else {
                notificationFactory.error('SelectEmployeeBeforeAssignWarning');
            }
        }

        function getPermissionOnTargetEmployeeSetting() {
            return targetEmployeeSettingFactory.getUserPermissionOnTargetEmployeeSetting().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getTargetDepartmentSetting(departmentId, isGroup, targetTypeId, showAll) {
            return targetEmployeeSettingFactory.getTargetDepartmentSetting(departmentId, isGroup, targetTypeId, showAll)
            .success(function (targetDepartments) {
                model.targetDepartmentSettings = targetDepartments;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getTargetEmployeeSetting(teamIds, employeeId, targetTypeId, showAll) {
            return targetEmployeeSettingFactory.getTargetEmployeeSetting(teamIds, employeeId, targetTypeId, showAll)
            .success(function (targetEmployeeSettings) {
                model.targetEmployeeSettings = targetEmployeeSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupDeleteTargetDepartmentSetting(targetTemplate, department) {
            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return targetTemplate.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                return targetEmployeeSettingFactory.deleteTargetDepartmentSetting(targetTemplate.TargetTemplateId, department.DepartmentId, department.IsGroup)
                    .success(function (response) {
                        if (response.IsSuccessful) {
                            var index = department.TargetTemplates.indexOf(targetTemplate);
                            department.TargetTemplates.splice(index, 1);
                            notificationFactory.success('DeletedSuccessfully');
                        } else {
                            notificationFactory.error('', response.Message);
                        }
                    }).error(function (response, status) {
                        errorFactory.errorHandle(status);
                    });
            })['finally'](function () {
                modalInstance = '';
            });
        }

        function openPopupDeleteTargetEmployeeSetting(targetTemplate, employee) {
            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return targetTemplate.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                return targetEmployeeSettingFactory.deleteTargetEmployeeSetting(targetTemplate.TargetTemplateId, employee.EmployeeId)
                    .success(function (response) {
                        if (response.IsSuccessful) {
                            var index = employee.TargetTemplates.indexOf(targetTemplate);
                            employee.TargetTemplates.splice(index, 1);
                            notificationFactory.success('DeletedSuccessfully');
                        } else {
                            notificationFactory.error('', response.Message);
                        }
                    }).error(function (response, status) {
                        errorFactory.errorHandle(status);
                    });
            })['finally'](function () {
                modalInstance = '';
            });
        }
    }
})(angular);