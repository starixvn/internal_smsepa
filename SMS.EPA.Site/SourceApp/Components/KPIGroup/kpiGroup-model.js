'use strict';
/*global kpiGroupModule*/

kpiGroupModule.factory('KPIGroupModel',
['$modal', 'kpiGroupFactory', 'notificationFactory', 'errorFactory',
function ($modal, kpiGroupFactory, notificationFactory, errorFactory) {
    var kpiGroupModel = {};

    kpiGroupModel.kpiGroups = {};

    kpiGroupModel.getKPIGroup = function (isActive) {
        return kpiGroupFactory.getKPIGroup(isActive)
        .success(function (kpiGroups) {
            kpiGroupModel.kpiGroups = kpiGroups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return kpiGroupModel;
} ]);