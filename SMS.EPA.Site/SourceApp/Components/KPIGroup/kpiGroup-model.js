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

    return kpiGroupModel;
} ]);