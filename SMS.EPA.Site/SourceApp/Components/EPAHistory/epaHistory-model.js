'use strict';
/*global epaHistoryModule*/

epaHistoryModule.factory('EPAHistoryModel', ['epaHistoryFactory', 'errorFactory',
function (epaHistoryFactory, errorFactory) {
    var epaHistoryModel = {};

    epaHistoryModel.epas = [];

    epaHistoryModel.getEPAHistory = function (periodId) {
        return epaHistoryFactory.getEPAHistory(periodId).success(function (epas) {
            epaHistoryModel.epas = epas;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return epaHistoryModel;
}]);