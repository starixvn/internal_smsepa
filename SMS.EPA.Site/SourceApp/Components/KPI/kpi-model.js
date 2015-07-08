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

    return kpiModel;
} ]);