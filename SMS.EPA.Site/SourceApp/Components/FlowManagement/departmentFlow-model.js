(function (flowMngModule) {
    'use strict';

    flowMngModule.factory('DeptFlowModel', deptFlowModel);

    deptFlowModel.$inject = ['$modal', 'FlowSettingModel', 'deptFlowFactory', 'notificationFactory', 'errorFactory'];

    function deptFlowModel($modal, FlowSettingModel, deptFlowFactory, notificationFactory, errorFactory) {
        var flowModel = {};

        flowModel.selfs = angular.copy(FlowSettingModel.selfs);
        flowModel.evaluators = angular.copy(FlowSettingModel.evaluators);
        flowModel.reviewers = angular.copy(FlowSettingModel.reviewers);
        flowModel.managements = angular.copy(FlowSettingModel.managements);
        flowModel.humanResources = angular.copy(FlowSettingModel.humanResources);
        flowModel.departmentSettings = [];
        flowModel.flowSettings = [];

        flowModel.getDepartmentFlow = function (flowTemplateId) {
            return deptFlowFactory.getDepartmentFlow(flowTemplateId).success(function (flows) {
                flowModel.flowSettings = flows;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.getDepartmentFlowById = function (department, flowTemplateId) {
            return deptFlowFactory.getDepartmentFlowById(department.DepartmentId, flowTemplateId).success(function (flows) {
                FlowSettingModel.resetCurrentFlow(flowModel);
                if (flows.length > 0) {
                    FlowSettingModel.successGetFlow(flows, flowModel);
                } else {
                    flowModel.selfs.employees = [{ NickName: 'Self', EmployeeId: -1 }];
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.validateDepartmentFlow = function (vm) {
            if (!vm.department || !vm.department.DepartmentId) {
                notificationFactory.error('DepartmentNotSelectedWarning');
                return false;
            } else if (vm.humanResources.employees.length === 0 || vm.evaluators.employees.length === 0 || vm.reviewers.employees.length === 0) {
                notificationFactory.error('SettingFlowWarning');
                return false;
            }

            return true;
        };

        flowModel.createDepartmentFlow = function (vm) {
            var model = {
                departmentId: vm.department.DepartmentId,
                flowTemplateId: vm.FlowTemplateId,
                isGroup: vm.department.DepartmentId <= 0,
                selfs: vm.selfs,
                evaluators: vm.evaluators,
                reviewers: vm.reviewers,
                managements: vm.managements,
                humanResources: vm.humanResources
            };

            return deptFlowFactory.createDepartmentFlow(model).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.openPopupDeleteDepartmentFlow = function (vm) {
            if (!vm.department || !vm.department.DepartmentId) {
                notificationFactory.error('', 'Please select a department!');
            } else {
                var modalInstance = $modal.open({
                    templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                    controller: 'ModalConfirmController as modalConfirmCtrl',
                    backdrop: 'static',
                    resolve: {
                        name: function () {
                            return '';
                        },
                        action: function () {
                            return "delete this department's setting";
                        }
                    },
                    windowClass: 'modal-confirm-center'
                });

                return modalInstance.result.then(function () {
                    deptFlowFactory.deleteDepartmentFlow(vm.department.DepartmentId, vm.FlowTemplateId).success(function (response) {
                        if (response.IsSuccessful) {
                            notificationFactory.success('DeletedSuccessfully');

                            vm.department = null;
                            FlowSettingModel.resetCurrentFlow(vm);
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
        };

        flowModel.getDepartmentHasSetting = function (flowTemplateId) {
            return deptFlowFactory.getDepartmentHasSetting(flowTemplateId).success(function (departments) {
                flowModel.departmentSettings = departments;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        return flowModel;
    }
})(angular.module('flowMngModule'));