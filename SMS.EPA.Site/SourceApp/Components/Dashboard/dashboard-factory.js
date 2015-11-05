'use strict';
/*global dashboardModule*/

dashboardModule.factory('dashboardFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getDashboard: function () {
            return $http.get(baseUrl + 'api/Dashboard/GetDashboard');
        }
    };
}]);