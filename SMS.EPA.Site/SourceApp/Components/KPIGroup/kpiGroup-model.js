'use strict';
/*global kpiGroupModule*/

kpiGroupModule.factory('KPIGroupModel',
['$modal', 'kpiGroupFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, kpiGroupFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var kpiGroupModel = {};

    kpiGroupModel.permission = 0;

    /// Validate KPI Group.
    kpiGroupModel.validateKPIGroup = function (kpiGroup) {
        if (!kpiGroup) {
            return false;
        }
        else {
            if (!kpiGroup.TargetTypeId) {
                notificationFactory.error("FormRequire");
                return false;
            }
            else if (!kpiGroup.Name) {
                notificationFactory.error("NameRequire");
                return false;
            }
        }

        return true;
    };

    /// Insert KPI Group.
    kpiGroupModel.openPopupCreateKPIGroup = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/KPIGroup/Templates/create.html',
            controller: "CreateKPIGroupController as createKPIGroupCtrl",
            backdrop: 'static',
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    kpiGroupModel.createKPIGroup = function (kpiGroup, modalInstance) {
        return kpiGroupFactory.createKPIGroup(kpiGroup)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreatedSuccessfully");
                modalInstance.close();
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Update KPI Group.
    kpiGroupModel.openPopupUpdateKPIGroup = function (vm, kpiGroup) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/KPIGroup/Templates/edit.html',
            controller: "UpdateKPIGroupController as updateKPIGroupCtrl",
            backdrop: 'static',
            resolve: {
                kpiGroup: function () {
                    return angular.copy(kpiGroup);
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    kpiGroupModel.updateKPIGroup = function (kpiGroup, modalInstance) {
        return kpiGroupFactory.updateKPIGroup(kpiGroup)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdateSuccessfully");
                modalInstance.close(kpiGroup);
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Update status KPI Group.
    kpiGroupModel.openPopupUpdateStatusKPIGroup = function (kpiGroup) {
        kpiGroup.IsActive = !kpiGroup.IsActive;

        var action = kpiGroup.IsActive ? "activate" : "deactivate",
            newKPIGroup = {
                KPIGroupId: kpiGroup.KPIGroupId,
                IsActive: kpiGroup.IsActive
            };

        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return kpiGroup.Name;
                },
                action: function () {
                    return action;
                }
            },
            windowClass: "modal-confirm-center"
        });

        var keyMessage = '';

        modalInstance.result.then(function () {
            kpiGroupFactory.updateStatusKPIGroup(newKPIGroup)
            .success(function (response) {
                if (response.IsSuccessful) {
                    keyMessage = action === "activate" ? "ActivationSuccess" : "DeactivationSuccess";
                    notificationFactory.success(keyMessage);
                }
                else {
                    kpiGroup.IsActive = !newKPIGroup.IsActive;
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                }
                else {
                    keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
                    kpiGroup.IsActive = !newKPIGroup.IsActive;
                    notificationFactory.error(keyMessage);
                }
            });
        }, function () {
            kpiGroup.IsActive = !newKPIGroup.IsActive;
        })['finally'](function () {
            modalInstance = '';
        });
    };

    /// Delete KPI Group.
    kpiGroupModel.openPopupDeleteKPIGroup = function (vm, kpiGroup) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return kpiGroup.Name;
                },
                action: function () {
                    return 'delete';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            kpiGroupFactory.deleteKPIGroup(kpiGroup.KPIGroupId).success(function (response) {
                if (response.IsSuccessful) {
                    var index = vm.kpiGroups.indexOf(kpiGroup);
                    vm.kpiGroups.splice(index, 1);
                    utilitiesFactory.searchOnPage(vm, vm.kpiGroups);

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

    /// Get user permission on KPI Group.
    kpiGroupModel.getUserPermissionOnKPIGroup = function () {
        return kpiGroupFactory.getUserPermissionOnKPIGroup()
        .success(function (permission) {
            kpiGroupModel.permission = parseInt(permission);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return kpiGroupModel;
} ]);