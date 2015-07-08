'use strict';
/*global kpiGroupModule*/

kpiGroupModule.factory('kpiGroupFactory', ['$http', function ($http) {
    return {
        getKPIGroup: function (isActive) {
            return $http.get('/api/KPIGroup/GetKPIGroup', { params: { isActive: isActive} });
        }
    };
} ]);