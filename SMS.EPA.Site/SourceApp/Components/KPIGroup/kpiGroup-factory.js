'use strict';
/*global kpiGroupModule*/

kpiGroupModule.factory('kpiGroupFactory', ['$http', function ($http) {
    return {
        createKPIGroup: function (kpiGroup) {
            return $http.post('/api/KPIGroup/CreateKPIGroup', kpiGroup);
        },
        updateKPIGroup: function (kpiGroup) {
            return $http.put('/api/KPIGroup/UpdateKPIGroup', kpiGroup);
        },
        updateStatusKPIGroup: function (kpiGroup) {
            return $http.put('/api/KPIGroup/UpdateStatusKPIGroup', kpiGroup);
        },
        deleteKPIGroup: function (kpiGroupId) {
            return $http['delete']('/api/KPIGroup/DeleteKPIGroup', { params: { kpiGroupId: kpiGroupId} });
        },
        getUserPermissionOnKPIGroup: function () {
            return $http.get('/api/KPIGroup/GetUserPermissionOnKPIGroup');
        }
    };
} ]);