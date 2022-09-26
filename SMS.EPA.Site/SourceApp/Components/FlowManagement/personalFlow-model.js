(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').factory('PersonalFlowModel', personalFlowModel);
    personalFlowModel.$inject = ['$uibModal', 'FlowSettingModel', 'personalFlowFactory', 'notificationFactory', 'errorFactory'];

    function personalFlowModel($uibModal, FlowSettingModel, personalFlowFactory, notificationFactory, errorFactory) {
        var flowModel = {};

        flowModel.selfs = angular.copy(FlowSettingModel.selfs);
        flowModel.evaluators = angular.copy(FlowSettingModel.evaluators);
        flowModel.reviewers = angular.copy(FlowSettingModel.reviewers);
        flowModel.managements = angular.copy(FlowSettingModel.managements);
        flowModel.humanResources = angular.copy(FlowSettingModel.humanResources);
        flowModel.defaultFlowSettings = [];
        flowModel.personalFlowSettings = [];
        flowModel.flowSettings = [];

        flowModel.getPersonalFlow = function (flowTemplateId) {
            return personalFlowFactory.getPersonalFlow(flowTemplateId).success(function (flows) {
                flowModel.flowSettings = flows;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.getPersonalFlowById = function (employeeId, flowTemplateId) {
            return personalFlowFactory.getPersonalFlowById(employeeId, flowTemplateId).success(function (flows) {
                FlowSettingModel.resetCurrentFlow(flowModel);
                FlowSettingModel.successGetFlow(flows, flowModel);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.validatePersonalFlow = function (vm) {
            if (vm.employeeIds.length === 0) {
                notificationFactory.error('EmployeeNotSelectedWarning');
                return false;
            } else if (vm.humanResources.employees.length === 0 || vm.evaluators.employees.length === 0 || vm.reviewers.employees.length === 0) {
                notificationFactory.error('SettingFlowWarning');
                return false;
            }

            return true;
        };

        flowModel.createPersonalFlow = function (vm) {
            var model = {
                employeeIds: vm.employeeIds,
                flowTemplateId: vm.FlowTemplateId,
                selfs: vm.selfs,
                evaluators: vm.evaluators,
                reviewers: vm.reviewers,
                managements: vm.managements,
                humanResources: vm.humanResources
            };

            return personalFlowFactory.createPersonalFlow(model).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.openPopupDeletePersonalFlow = function (vm) {
            var index = -1;

            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return '';
                    },
                    action: function () {
                        return "delete this personal's setting";
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                personalFlowFactory.deletePersonalFlow(vm.employeeId, vm.FlowTemplateId).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success('DeletedSuccessfully');
                        index = vm.personalFlowSettings.map(function (e) { return e.EmployeeId; }).indexOf(vm.employeeId);

                        if (index !== -1) {
                            vm.defaultFlowSettings.push(vm.personalFlowSettings[index]);
                            vm.personalFlowSettings.splice(index, 1);
                        }

                        vm.employeeId = null;
                        vm.showDeleteButton = false;
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
        };

        flowModel.getEmployeeFlowSetting = function (flowTemplateId, teamIds, isMonthly) {
            return personalFlowFactory.getEmployeeFlowSetting(flowTemplateId, teamIds, isMonthly).success(function (data) {
                flowModel.defaultFlowSettings = data.DefaultFlowSettings;
                flowModel.personalFlowSettings = data.PersonalFlowSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        return flowModel;
    }
})(angular);