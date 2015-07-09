'use strict';
/*global kpiGroupModule*/

kpiGroupModule.factory('kpiGroupFactory', ['$http', function ($http) {
    return {
        getKPIGroup: function (targetTypeId, isActive) {
            return $http.get('/api/KPIGroup/GetKPIGroup', { params: { targetTypeId: targetTypeId, isActive: isActive} });
        },
        insertKPIGroup: function (kpiGroup) {
            return $http.post('/api/KPIGroup/InsertKPIGroup', kpiGroup);
        }
    };
} ]);