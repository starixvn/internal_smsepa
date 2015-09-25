'use strict';
/*global reportModule*/

reportModule.factory('reportFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEPASummaryReport: function (periodIds, departmentId) {
            return $http.get(baseUrl + 'api/Report/GetEPASummary',
                {
                    params: {
                        periodIds: periodIds,
                        departmentId: departmentId
                    }
                });
        }
    };
}]);