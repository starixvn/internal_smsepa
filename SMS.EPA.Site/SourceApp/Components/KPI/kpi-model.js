'use strict';
/*global kpiModule*/

kpiModule.factory('KPIModel',
['$modal', 'kpiFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, kpiFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var kpiModel = {};

    kpiModel.kpis = [];

    kpiModel.getKPI = function (targetTypeId, kpiGroupId, isActive) {
        return kpiFactory.getKPI(targetTypeId, kpiGroupId, isActive)
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
                notificationFactory.success("UpdateSuccessfully");
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

    /// Update required evaluate KPI.
    kpiModel.openPopupUpdateRequiredEvaluateKPI = function (kpi) {
        kpi.IsRequiredEvaluate = !kpi.IsRequiredEvaluate;

        var action = kpi.IsRequiredEvaluate ? "activate required evaluate" : "deactivate required evaluate",
            newKPI = {
                KPIId: kpi.KPIId,
                IsRequiredEvaluate: kpi.IsRequiredEvaluate
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
            kpiFactory.updateRequiredEvaluateKPI(newKPI)
            .success(function (response) {
                if (response.IsSuccessful) {
                    keyMessage = action === "activate required evaluate" ? "ActivationSuccess" : "DeactivationSuccess";
                    notificationFactory.success(keyMessage);
                }
                else {
                    kpi.IsRequiredEvaluate = !newKPI.IsRequiredEvaluate;
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                if (status === 405) {
                    errorFactory.errorHandle(status);
                }
                else {
                    keyMessage = action === "activate required evaluate" ? "ActivationFail" : "DeactivationFail";
                    kpi.IsRequiredEvaluate = !newKPI.IsRequiredEvaluate;
                    notificationFactory.error(keyMessage);
                }
            });
        }, function () {
            kpi.IsRequiredEvaluate = !newKPI.IsRequiredEvaluate;
        })['finally'](function () {
            modalInstance = '';
        });
    };

    /// Delete KPI.
    kpiModel.openPopupDeleteKPI = function (vm, kpi) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return kpi.Name;
                },
                action: function () {
                    return 'delete';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            kpiFactory.deleteKPI(kpi.KPIId).success(function (response) {
                if (response.IsSuccessful) {
                    var index = vm.kpis.indexOf(kpi);
                    vm.kpis.splice(index, 1);
                    utilitiesFactory.searchOnPage(vm, vm.kpis);

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

    return kpiModel;
} ]);