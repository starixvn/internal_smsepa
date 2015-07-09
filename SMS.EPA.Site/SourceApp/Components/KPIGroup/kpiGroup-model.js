'use strict';
/*global kpiGroupModule*/

kpiGroupModule.factory('KPIGroupModel',
['$modal', 'kpiGroupFactory', 'notificationFactory', 'errorFactory',
function ($modal, kpiGroupFactory, notificationFactory, errorFactory) {
    var kpiGroupModel = {};

    kpiGroupModel.kpiGroups = [];

    kpiGroupModel.getKPIGroup = function (targetTypeId, isActive) {
        return kpiGroupFactory.getKPIGroup(targetTypeId, isActive)
        .success(function (kpiGroups) {
            kpiGroupModel.kpiGroups = kpiGroups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

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

    kpiGroupModel.insertKPIGroup = function (kpiGroup, modalInstance) {
        return kpiGroupFactory.insertKPIGroup(kpiGroup)
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

    return kpiGroupModel;
} ]);