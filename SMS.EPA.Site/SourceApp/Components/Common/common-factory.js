'use strict';
/*global commonModule*/

commonModule.factory('commonFactory', ['$http', function ($http) {
    return {
        getTargetType: function (isActive) {
            return $http.get('/api/Common/GetTargetType', { params: { isActive: isActive} });
        },
        getSystemRoles: function (exceptNone) {
            return $http.get('/api/Common/GetSystemRoles', { params: { exceptNone: exceptNone } });
        },
        getKPIGroup: function (targetTypeId, isActive) {
            return $http.get('/api/Common/GetKPIGroup', { params: { targetTypeId: targetTypeId, isActive: isActive} });
        },
        getKPIGroupById: function (kpiGroupId) {
            return $http.get('/api/Common/GetKPIGroupById', { params: { kpiGroupId: kpiGroupId} });
        }
    };
} ]);