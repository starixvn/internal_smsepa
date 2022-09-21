(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaHistoryModule').factory('epaHistoryFactory', epaHistoryFactory);
    epaHistoryFactory.$inject = ['$http', 'baseUrl'];

    function epaHistoryFactory($http, baseUrl) {
        return {
            checkHasOnlyEmployeeRole: checkHasOnlyEmployeeRole,
            getEPAHistory: getEPAHistory,
            getMonthlyEPAHistory: getMonthlyEPAHistory
        };

        function checkHasOnlyEmployeeRole() {
            return $http.get(baseUrl + 'api/EPAHistory/HasOnlyEmployeeRole');
        }

        function getEPAHistory(periodId, teamIds, isLocal, employeeId) {
            return $http.get(baseUrl + 'api/EPAHistory/GetEPAHistory', {
                params: { periodId, teamIds, isLocal, employeeId }
            });
        }

        function getMonthlyEPAHistory(periodId, employeeId) {
            return $http.get(baseUrl + 'api/EPAHistory/GetMonthlyEPAHistory', { params: { periodId, employeeId } });
        }
    }
})(angular);