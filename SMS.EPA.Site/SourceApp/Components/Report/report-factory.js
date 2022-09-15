(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').factory('reportFactory', reportFactory);
    reportFactory.$inject = ['$http', 'baseUrl'];

    function reportFactory($http, baseUrl) {
        return {
            getAllPeriodIncludeImport: getAllPeriodIncludeImport,
            getEmployeeByActorId: getEmployeeByActorId,
            getEPADetailReport: getEPADetailReport,
            getEPASummaryReport: getEPASummaryReport,
            getMonthlyEPADetailReport: getMonthlyEPADetailReport,
            getMonthlyEPASummaryReport: getMonthlyEPASummaryReport
        };

        function getAllPeriodIncludeImport() {
            return $http.get(baseUrl + 'api/Report/GetAllPeriodIncludeImport');
        }

        function getEmployeeByActorId(query, periodIds, teamIds, isLocal) {
            return $http.get(baseUrl + 'api/Report/GetEmployeeByActorId', {
                params: { query, periodIds, teamIds, isLocal }
            });
        }

        function getEPADetailReport(periodId, teamIds, isLocal, employeeId, phaseIds, sortColumnName, sortDirection) {
            return $http.get(baseUrl + 'api/Report/GetEPADetailReport', {
                params: { periodId, teamIds, isLocal, employeeId, phaseIds, sortColumnName, sortDirection }
            });
        }

        function getEPASummaryReport(periodIds, teamIds, isLocal, employeeId, sortColumnName, sortDirection) {
            return $http.get(baseUrl + 'api/Report/GetEPASummaryReport', {
                params: { periodIds, teamIds, isLocal, employeeId, sortColumnName, sortDirection }
            });
        }

        function getMonthlyEPADetailReport(periodId, teamIds, isLocal, phaseIds, sortColumnName, sortDirection) {
            return $http.get(baseUrl + 'api/Report/GetMonthlyEPADetailReport', {
                params: { periodId, teamIds, isLocal, phaseIds, sortColumnName, sortDirection }
            });
        }

        function getMonthlyEPASummaryReport(periodIds, teamIds, isLocal, sortColumnName, sortDirection) {
            return $http.get(baseUrl + 'api/Report/GetMonthlyEPASummaryReport', {
                params: { periodIds, teamIds, isLocal, sortColumnName, sortDirection }
            });
        }
    }
})(angular);