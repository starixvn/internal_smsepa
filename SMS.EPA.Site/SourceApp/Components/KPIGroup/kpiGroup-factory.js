'use strict';
/*global kpiGroupModule*/

kpiGroupModule.factory('kpiGroupFactory', ['$http', function ($http) {
    return {
        getKPIGroup: function (targetTypeId, isActive) {
            return $http.get('/api/KPIGroup/GetKPIGroup', { params: { targetTypeId: targetTypeId, isActive: isActive} });
        }
    };
} ]);