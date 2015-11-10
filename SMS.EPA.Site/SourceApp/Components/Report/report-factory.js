'use strict';
/*global reportModule*/

reportModule.factory('reportFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getAllPeriodIncludeImport: function () {
            return $http.get(baseUrl + 'api/Report/GetAllPeriodIncludeImport');
        },
        getEPASummaryReport: function (periodIds, departmentId, isLocal, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetEPASummaryReport',
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
        }
    };
}]);