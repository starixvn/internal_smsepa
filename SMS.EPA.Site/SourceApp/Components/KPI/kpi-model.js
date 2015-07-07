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

    return kpiModel;
} ]);