'use strict';
/*global reportModule*/

reportModule.factory('reportFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEPASummaryReport: function (periodIds, departmentId, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetEPASummary',
                {
                    params: {
                        periodIds: periodIds,
                        departmentId: departmentId,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        },
        getEPADetailReport: function (periodId, departmentId, pageIndex, itemsPerPage) {
            return $http.get(baseUrl + 'api/Report/GetEPADetail',
                {
                    params: {
                        periodId: periodId,
                        departmentId: departmentId,
                        pageIndex: pageIndex,
                        itemsPerPage: itemsPerPage
                    }
                });
        }
    };
}]);