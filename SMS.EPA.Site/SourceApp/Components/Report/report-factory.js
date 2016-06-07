'use strict';
/*global reportModule*/

reportModule.factory('reportFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        requestExportEPA: function (epaId) {
            return $http.get(baseUrl + 'ExportPDF/RequestExportEPA/', { params: { epaId: epaId } });
        },
        requestExportSemiEPA: function (epaId) {
            return $http.get(baseUrl + 'ExportPDF/RequestExportSemiEPA/', { params: { epaId: epaId } });
        },
        getAllPeriodIncludeImport: function () {
            return $http.get(baseUrl + 'api/Report/GetAllPeriodIncludeImport');
        },
        getEPASummaryReport: function (periodIds, departmentId, isLocal, sortColumnName, sortDirection, pageIndex, itemsPerPage) {
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
        },
        getMonthlyEPASummaryReport: function (periodIds, departmentId, isLocal, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetMonthlyEPASummaryReport',
                {
                    params: {
                        periodIds: periodIds,
                        departmentId: departmentId,
                        isLocal: isLocal,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        },
        getEPADetailReport: function (periodId, departmentId, isLocal, phaseIds, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetEPADetailReport',
                {
                    params: {
                        periodId: periodId,
                        departmentId: departmentId,
                        isLocal: isLocal,
                        phaseIds: phaseIds,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        },
        getMonthlyEPADetailReport: function (periodId, departmentId, isLocal, phaseIds, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetMonthlyEPADetailReport',
                {
                    params: {
                        periodId: periodId,
                        departmentId: departmentId,
                        isLocal: isLocal,
                        phaseIds: phaseIds,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        }
    };
}]);