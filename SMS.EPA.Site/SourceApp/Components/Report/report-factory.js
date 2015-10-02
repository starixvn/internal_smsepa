'use strict';
/*global reportModule*/

reportModule.factory('reportFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEPASummaryReport: function (periodIds, departmentId, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetEPASummaryReport',
                {
                    params: {
                        periodIds: periodIds,
                        departmentId: departmentId,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        },
        getEPADetailReport: function (periodId, departmentId, phaseIds, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetEPADetailReport',
                {
                    params: {
                        periodId: periodId,
                        departmentId: departmentId,
                        phaseIds: phaseIds,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        }
    };
}]);