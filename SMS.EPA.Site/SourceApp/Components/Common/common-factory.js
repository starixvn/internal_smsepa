'use strict';
/*global commonModule*/

commonModule.factory('commonFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getTargetType: function (isActive) {
            return $http.get(baseUrl + 'api/Common/GetTargetType', { params: { isActive: isActive} });
        },
        getSystemRoles: function (exceptNone) {
            return $http.get(baseUrl + 'api/Common/GetSystemRoles', { params: { exceptNone: exceptNone} });
        },
        getKPI: function (targetTypeId, kpiGroupId, isActive, isRequiredEvaluate) {
            return $http.get(baseUrl + 'api/Common/GetKPI', { params: {
                targetTypeId: targetTypeId,
                kpiGroupId: kpiGroupId,
                isActive: isActive,
                isRequiredEvaluate: isRequiredEvaluate
            }
            });
        },
        getKPIGroup: function (targetTypeId, isActive) {
            return $http.get(baseUrl + 'api/Common/GetKPIGroup', { params: { targetTypeId: targetTypeId, isActive: isActive} });
        },
        getKPIGroupById: function (kpiGroupId) {
            return $http.get(baseUrl + 'api/Common/GetKPIGroupById', { params: { kpiGroupId: kpiGroupId} });
        }
    };
} ]);