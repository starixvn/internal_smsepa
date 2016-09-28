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
            getMonthlyEPASummaryReport: getMonthlyEPASummaryReport,
            requestExportEPA: requestExportEPA,
            requestExportSemiEPA: requestExportSemiEPA
        };

        function getAllPeriodIncludeImport() {
            return $http.get(baseUrl + 'api/Report/GetAllPeriodIncludeImport');
        }

        function getEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetEPADetailReport',
                {
                    params: {
                        periodId: periodId,
                        departmentId: departmentId,
                        isLocal: isLocal,
                        phaseIds: phaseIds,
                        sortColumnName: sortColumnName,
                        sortDirection: sortDirection,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        }

        function getEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetEPASummaryReport',
                {
                    params: {
                        periodIds: periodIds,
                        departmentId: departmentId,
                        isLocal: isLocal,
                        sortColumnName: sortColumnName,
                        sortDirection: sortDirection,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        }

        function getMonthlyEPADetailReport(periodId, departmentId, isLocal, phaseIds, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetMonthlyEPADetailReport',
                {
                    params: {
                        periodId: periodId,
                        departmentId: departmentId,
                        isLocal: isLocal,
                        phaseIds: phaseIds,
                        sortColumnName: sortColumnName,
                        sortDirection: sortDirection,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        }

        function getMonthlyEPASummaryReport(periodIds, departmentId, isLocal, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetMonthlyEPASummaryReport',
                {
                    params: {
                        periodIds: periodIds,
                        departmentId: departmentId,
                        isLocal: isLocal,
                        sortColumnName: sortColumnName,
                        sortDirection: sortDirection,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        }

        function requestExportEPA(epaId) {
            return $http.get(baseUrl + 'ExportPDF/RequestExportEPA/', { params: { epaId: epaId } });
        }

        function requestExportSemiEPA(epaId) {
            return $http.get(baseUrl + 'ExportPDF/RequestExportSemiEPA/', { params: { epaId: epaId } });
        }
    }
})(angular);