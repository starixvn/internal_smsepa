'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.factory('monthlyPeriodFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getMonthlyPeriod: function (fromDate, toDate) {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetMonthlyPeriod',
                {
                    params: {
                        fromDate: fromDate,
                        toDate: toDate
                    }
                });
        },
        getMonthlyPeriodById: function (periodId) {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetMonthlyPeriodById', { params: { periodId: periodId } });
        },
        createMonthlyPeriod: function (period) {
            return $http.post(baseUrl + 'api/MonthlyPeriod/CreateMonthlyPeriod', period);
        },
        updateBasicInfoMonthlyPeriod: function (period) {
            return $http.put(baseUrl + 'api/MonthlyPeriod/UpdateBasicInfoMonthlyPeriod', period);
        },
        getPermissionOnMonthlyPeriod: function () {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetUserPermissionOnMonthlyPeriod');
        }
    };
}]);