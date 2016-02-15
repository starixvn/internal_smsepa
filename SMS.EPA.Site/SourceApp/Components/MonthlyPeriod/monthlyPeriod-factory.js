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
        createMonthlyPeriod: function (period) {
            return $http.post(baseUrl + 'api/MonthlyPeriod/CreateMonthlyPeriod', period);
        },
        getPermissionOnMonthlyPeriod: function () {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetUserPermissionOnMonthlyPeriod');
        }
    };
}]);