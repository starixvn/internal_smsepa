'use strict';
/*global kpiModule*/

kpiModule.factory('kpiFactory', ['$http', function ($http) {
    return {
        getKPI: function (kpiGroupId, isActive) {
            return $http.get('/api/KPI/GetKPI', { params: { kpiGroupId: kpiGroupId, isActive: isActive} });
        }
    };
} ]);