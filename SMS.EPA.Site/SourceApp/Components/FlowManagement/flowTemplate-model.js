(function (flowMngModule) {
    'use strict';

    flowMngModule.factory('FlowTemplateModel', flowTemplateModel);

    flowTemplateModel.$inject = ['$modal', 'flowTemplateFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function flowTemplateModel($modal, flowTemplateFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var flowModel = {};

        flowModel.flowTemplates = [];
        flowModel.flowTemplate = {};
        flowModel.permission = 0;

        flowModel.getFlowTemplate = function (targetTypeId, isActive) {
            return flowTemplateFactory.getFlowTemplate(targetTypeId, isActive).success(function (flowTemplates) {
                flowModel.flowTemplates = flowTemplates;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.getFlowTemplateById = function (flowTemplateId) {
            return flowTemplateFactory.getFlowTemplateById(flowTemplateId).success(function (flowTemplate) {
                flowModel.flowTemplate = flowTemplate;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.openPopupCreateFlowTemplate = function (vm) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/FlowManagement/Templates/modalCreateFlowTemplate.html',
                controller: "CreateFlowTemplateController as createFlowTemplateCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        }

        flowModel.validateFlowTemplate = function (flowTemplate) {
            if (!flowTemplate) {
                return false;
            } else if (!flowTemplate.Name) {
                notificationFactory.error('NameRequired');
                return false;
            } else if (!flowTemplate.TargetTypeId) {
                notificationFactory.error('TargetTypeRequired');
                return false;
            }

            return true;
        };

        flowModel.createFlowTemplate = function (flowTemplate, modalInstance) {
            return flowTemplateFactory.createFlowTemplate(flowTemplate).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function () {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.updateFlowTemplate = function (flowTemplate) {
            return flowTemplateFactory.updateFlowTemplate(flowTemplate).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function () {
                errorFactory.errorHandle(status);
            });
        };

        flowModel.openPopupUpdateStatusFlowTemplate = function (flowTemplate) {
            flowTemplate.IsActive = !flowTemplate.IsActive;

            var action = flowTemplate.IsActive ? 'activate' : 'deactivate',
                newFlowTemplate = {
                    flowTemplateId: flowTemplate.FlowTemplateId,
                    IsActive: flowTemplate.IsActive
                };

            var modalInstance = $modal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return flowTemplate.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            var keyMessage = '';

            modalInstance.result.then(function () {
                flowTemplateFactory.updateStatusFlowTemplate(newFlowTemplate).success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === 'activate' ? 'ActivationSuccess' : 'DeactivationSuccess';
                        notificationFactory.success(keyMessage);
                    } else {
                        flowTemplate.IsActive = !newFlowTemplate.IsActive;
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                        flowTemplate.IsActive = !newFlowTemplate.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                });
            }, function () {
                flowTemplate.IsActive = !newFlowTemplate.IsActive;
            })['finally'](function () {
                modalInstance = '';
            });
        };

        flowModel.openPopupDeleteFlowTemplate = function (vm, flowTemplate) {
            var modalInstance = $modal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return flowTemplate.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                flowTemplateFactory.deleteFlowTemplate(flowTemplate.FlowTemplateId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.flowTemplates.indexOf(flowTemplate);
                        vm.flowTemplates.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.flowTemplates);

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
        };

        flowModel.getUserPermissionOnFlowManagement = function () {
            return flowTemplateFactory.getUserPermissionOnFlowManagement().success(function (permission) {
                flowModel.permission = parseInt(permission);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        return flowModel;
    }
})(angular.module('flowMngModule'));