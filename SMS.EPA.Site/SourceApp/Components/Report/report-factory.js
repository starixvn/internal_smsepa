(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').factory('reportFactory', reportFactory);
    reportFactory.$inject = ['$http', 'baseUrl'];

    function reportFactory($http, baseUrl) {
        return {
            getAllPeriodIncludeImport: getAllPeriodIncludeImport,
            getEPADetailReport: getEPADetailReport,
            getEPASummaryReport: getEPASummaryReport,
            getMonthlyEPADetailReport: getMonthlyEPADetailReport,
            getMonthlyEPASummaryReport: getMonthlyEPASummaryReport
        };

        function getAllPeriodIncludeImport() {
            return $http.get(baseUrl + 'api/Report/GetAllPeriodIncludeImport');
        }

        function getEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection) {
            return $http.get(baseUrl + 'api/Report/GetEPADetailReport', {
                params: {
                    periodId: periodId,
                    departmentId: departmentId,
                    isLocal: isLocal,
                    phaseIds: phaseIds,
                    sortColumnName: sortColumnName,
                    sortDirection: sortDirection
                }
            });
        }

        function getEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection) {
            return $http.get(baseUrl + 'api/Report/GetEPASummaryReport', {
                params: {
                    periodIds: periodIds,
                    departmentId: departmentId,
                    isLocal: isLocal,
                    sortColumnName: sortColumnName,
                    sortDirection: sortDirection
                }
            });
        }

        function getMonthlyEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection) {
            return $http.get(baseUrl + 'api/Report/GetMonthlyEPADetailReport', {
                params: {
                    periodId: periodId,
                    departmentId: departmentId,
                    isLocal: isLocal,
                    phaseIds: phaseIds,
                    sortColumnName: sortColumnName,
                    sortDirection: sortDirection
                }
            });
        }

        function getMonthlyEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection) {
            return $http.get(baseUrl + 'api/Report/GetMonthlyEPASummaryReport', {
                params: {
                    periodIds: periodIds,
                    departmentId: departmentId,
                    isLocal: isLocal,
                    sortColumnName: sortColumnName,
                    sortDirection: sortDirection
                }
            });
        }
    }
})(angular);