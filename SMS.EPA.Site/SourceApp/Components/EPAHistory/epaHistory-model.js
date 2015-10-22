'use strict';
/*global epaHistoryModule*/

epaHistoryModule.factory('EPAHistoryModel', ['epaHistoryFactory', 'errorFactory',
function (epaHistoryFactory, errorFactory) {
    var epaHistoryModel = {};

    epaHistoryModel.epas = [];
    epaHistoryModel.onlyEmployeeRole = false;

    epaHistoryModel.getEPAHistory = function (periodId, employeeId) {
        return epaHistoryFactory.getEPAHistory(periodId, employeeId).success(function (epas) {
            epaHistoryModel.epas = epas;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    epaHistoryModel.checkHasOnlyEmployeeRole = function () {
        return epaHistoryFactory.checkHasOnlyEmployeeRole().success(function (onlyEmployeeRole) {
            epaHistoryModel.onlyEmployeeRole = onlyEmployeeRole;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return epaHistoryModel;
}]);