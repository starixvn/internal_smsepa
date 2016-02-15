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
        getPermissionOnMonthlyPeriod: function () {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetUserPermissionOnMonthlyPeriod');
        }
    };
}]);