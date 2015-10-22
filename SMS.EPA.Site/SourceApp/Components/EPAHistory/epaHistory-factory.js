'use strict';
/*global epaHistoryModule*/

epaHistoryModule.factory('epaHistoryFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEPAHistory: function (periodId, employeeId) {
            return $http.get(baseUrl + 'api/EPAHistory/GetEPAHistory', { params: { periodId: periodId, employeeId: employeeId } });
        },
        checkHasOnlyEmployeeRole: function () {
            return $http.get(baseUrl + 'api/EPAHistory/HasOnlyEmployeeRole');
        }
    };
}]);