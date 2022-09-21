(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaHistoryModule').factory('EPAHistoryModel', epaHistoryModel);
    epaHistoryModel.$inject = ['epaHistoryFactory', 'errorFactory'];

    function epaHistoryModel(epaHistoryFactory, errorFactory) {
        var epas = [],
            onlyEmployeeRole = false;

        var model = {
            epas: epas,
            onlyEmployeeRole: onlyEmployeeRole,
            checkHasOnlyEmployeeRole: checkHasOnlyEmployeeRole,
            getEPAHistory: getEPAHistory,
            getMonthlyEPAHistory: getMonthlyEPAHistory
        };

        return model;

        function checkHasOnlyEmployeeRole() {
            return epaHistoryFactory.checkHasOnlyEmployeeRole().success(function (onlyEmployeeRole) {
                model.onlyEmployeeRole = onlyEmployeeRole;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPAHistory(periodId, teamIds, isLocal, employeeId) {
            return epaHistoryFactory.getEPAHistory(periodId, teamIds, isLocal, employeeId).success(function (epas) {
                model.epas = epas;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMonthlyEPAHistory(periodId, employeeId) {
            return epaHistoryFactory.getMonthlyEPAHistory(periodId, employeeId).success(function (epas) {
                model.epas = epas;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);