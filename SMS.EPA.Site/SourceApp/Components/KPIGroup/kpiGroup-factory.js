'use strict';
/*global kpiGroupModule*/

kpiGroupModule.factory('kpiGroupFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getKPIGroup: function (isActive) {
            return $http.get(baseUrl + 'api/KPIGroup/GetKPIGroup', { params: { isActive: isActive } });
        },
        getKPIGroupById: function (kpiGroupId) {
            return $http.get(baseUrl + 'api/KPIGroup/GetKPIGroupById', { params: { kpiGroupId: kpiGroupId } });
        },
        createKPIGroup: function (kpiGroup) {
            return $http.post(baseUrl + 'api/KPIGroup/CreateKPIGroup', kpiGroup);
        },
        updateKPIGroup: function (kpiGroup) {
            return $http.put(baseUrl + 'api/KPIGroup/UpdateKPIGroup', kpiGroup);
        },
        updateStatusKPIGroup: function (kpiGroup) {
            return $http.put(baseUrl + 'api/KPIGroup/UpdateStatusKPIGroup', kpiGroup);
        },
        deleteKPIGroup: function (kpiGroupId) {
            return $http['delete'](baseUrl + 'api/KPIGroup/DeleteKPIGroup', { params: { kpiGroupId: kpiGroupId} });
        },
        getUserPermissionOnKPIGroup: function () {
            return $http.get(baseUrl + 'api/KPIGroup/GetUserPermissionOnKPIGroup');
        }
    };
} ]);