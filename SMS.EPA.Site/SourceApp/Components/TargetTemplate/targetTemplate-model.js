'use strict';
/*global targetTemplateModule*/

targetTemplateModule.factory('TargetTemplateModel',
['$modal', 'targetTemplateFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory',
function ($modal, targetTemplateFactory, errorFactory, notificationFactory, utilitiesFactory) {
    var targetTemplateModel = {};

    targetTemplateModel.targetTemplates = [];
    targetTemplateModel.targetTemplate = {};

    targetTemplateModel.getTargetTemplate = function (targetTypeId, isActive) {
        return targetTemplateFactory.getTargetTemplate(targetTypeId, isActive).success(function (targetTemplates) {
            targetTemplateModel.targetTemplates = targetTemplates;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    targetTemplateModel.validateTargetTemplate = function (targetTemplate) {
        if (!targetTemplate) {
            return false;
        }
        else {
            if (!targetTemplate.Name) {
                notificationFactory.error("NameRequire");
                return false;
            }
            else if (!targetTemplate.TargetTypeId) {
                notificationFactory.error("TargetTypeRequired");
                return false;
            }
        }

        return true;
    };

    targetTemplateModel.createTargetTemplate = function (targetTemplate) {
        return targetTemplateFactory.createTargetTemplate(targetTemplate).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreateSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function () {
            errorFactory.errorHandle(status);
        });
    };

    targetTemplateModel.openPopupUpdateStatusTargetTemplate = function (targetTemplate) {
        targetTemplate.IsActive = !targetTemplate.IsActive;

        var action = targetTemplate.IsActive ? "activate" : "deactivate",
        newTargetTemplate = {
            TargetTemplateId: targetTemplate.TargetTemplateId,
            IsActive: targetTemplate.IsActive
        };

        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return targetTemplate.Name;
                },
                action: function () {
                    return action;
                }
            },
            windowClass: "modal-confirm-center"
        });

        var keyMessage = '';

        modalInstance.result.then(function () {
            targetTemplateFactory.updateStatusTargetTemplate(newTargetTemplate)
            .success(function (response) {
                if (response.IsSuccessful) {
                    keyMessage = action === "activate" ? "ActivationSuccess" : "DeactivationSuccess";
                    notificationFactory.success(keyMessage);
                }
                else {
                    keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
                    targetTemplate.IsActive = !newTargetTemplate.IsActive;
                    notificationFactory.error(keyMessage);
                }
            })
            .error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                }
                else {
                    keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
                    targetTemplate.IsActive = !newTargetTemplate.IsActive;
                    notificationFactory.error(keyMessage);
                }
            });
        }, function () {
            targetTemplate.IsActive = !newTargetTemplate.IsActive;
        })['finally'](function () {
            modalInstance = '';
        });
    };

    targetTemplateModel.getTargetTemplateById = function (targetTemplateId) {
        return targetTemplateFactory.getTargetTemplateById(targetTemplateId).success(function (targetTemplate) {
            targetTemplateModel.targetTemplate = targetTemplate;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    targetTemplateModel.updateTargetTemplate = function (targetTemplate) {
        return targetTemplateFactory.updateTargetTemplate(targetTemplate).success(function (response) {
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

    targetTemplateModel.openPopupDeleteTargetTemplate = function (vm, targetTemplate) {
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
            targetTemplateFactory.deleteTargetTemplate(targetTemplate.TargetTemplateId).success(function (response) {
                if (response.IsSuccessful) {
                    var index = vm.targetTemplates.indexOf(targetTemplate);
                    vm.targetTemplates.splice(index, 1);
                    utilitiesFactory.searchOnPage(vm, vm.targetTemplates);
                    notificationFactory.success("DeletedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    return targetTemplateModel;
} ]);