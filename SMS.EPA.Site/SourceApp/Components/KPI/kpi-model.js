'use strict';
/*global kpiModule*/

kpiModule.factory('KPIModel',
['$modal', 'kpiFactory', 'notificationFactory', 'errorFactory',
function ($modal, kpiFactory, notificationFactory, errorFactory) {
    var kpiModel = {};

    kpiModel.kpis = {};

    kpiModel.getKPI = function (kpiGroupId, isActive) {
        return kpiFactory.getKPI(kpiGroupId, isActive)
        .success(function (kpis) {
            kpiModel.kpis = kpis;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    kpiModel.validateKPI = function (kpi, currentChoseKPIGroups) {
        if (!kpi) {
            return false;
        }
        else {
            if (!kpi.Name) {
                notificationFactory.error("TitleRequire");
                return false;
            }
            else if (currentChoseKPIGroups.length === 0) {
                notificationFactory.error("GroupRequire");
                return false;
            }
        }

        return true;
    };

    /// Insert KPI.
    kpiModel.openPopupCreateKPI = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/KPI/Templates/create.html',
            controller: "CreateKPIController as createKPICtrl",
            backdrop: 'static',
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    kpiModel.insertKPI = function (vm, isCreateAndContinued, modalInstance) {
        return kpiFactory.insertKPI(vm.kpi)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreatedSuccessfully");

                if (isCreateAndContinued) {
                    vm.kpi = {
                        Name: '',
                        KPIGroupIds: '',
                        HasComment: false,
                        IsRequiredComment: false,
                        IsRequiredEvaluate: false,
                        Description: ''
                    };
                }
                else {
                    modalInstance.close();
                }
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Update KPI
    kpiModel.openPopupUpdateKPI = function (vm, kpi) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/KPI/Templates/edit.html',
            controller: "UpdateKPIController as updateKPICtrl",
            backdrop: 'static',
            resolve: {
                kpi: function () {
                    return angular.copy(kpi);
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    kpiModel.updateKPI = function (kpi, modalInstance) {
        return kpiFactory.updateKPI(kpi)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("DataChangeSuccessfully");
                modalInstance.close(kpi);
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Update status KPI.
    kpiModel.openPopupUpdateStatusKPI = function (kpi) {
        kpi.IsActive = !kpi.IsActive;

        var action = kpi.IsActive ? "activate" : "deactivate",
            newKPI = {
                KPIId: kpi.KPIId,
                IsActive: kpi.IsActive
            };

        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return kpi.Name;
                },
                action: function () {
                    return action;
                }
            },
            windowClass: "modal-confirm-center"
        });

        var keyMessage = '';

        modalInstance.result.then(function () {
            kpiFactory.updateStatusKPI(newKPI)
            .success(function (response) {
                if (response.IsSuccessful) {
                    keyMessage = action === "activate" ? "ActivationSuccess" : "DeactivationSuccess";
                    notificationFactory.success(keyMessage);
                }
                else {
                    kpi.IsActive = !newKPI.IsActive;
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                }
                else {
                    keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
                    kpi.IsActive = !newKPI.IsActive;
                    notificationFactory.error(keyMessage);
                }
            });
        }, function () {
            kpi.IsActive = !newKPI.IsActive;
        })['finally'](function () {
            modalInstance = '';
        });
    };

    return kpiModel;
} ]);