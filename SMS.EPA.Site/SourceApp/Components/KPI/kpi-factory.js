'use strict';
/*global kpiModule*/

kpiModule.factory('kpiFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getKPI: function (targetTypeId, kpiGroupId, isActive) {
            return $http.get(baseUrl + 'api/KPI/GetKPI', { params: { targetTypeId: targetTypeId, kpiGroupId: kpiGroupId, isActive: isActive} });
        },
        createKPI: function (kpi) {
            return $http.post(baseUrl + 'api/KPI/CreateKPI', kpi);
        },
        updateKPI: function (kpi) {
            return $http.put(baseUrl + 'api/KPI/UpdateKPI', kpi);
        },
        updateStatusKPI: function (kpi) {
            return $http.put(baseUrl + 'api/KPI/UpdateStatusKPI', kpi);
        },
        updateRequiredEvaluateKPI: function (kpi) {
            return $http.put(baseUrl + 'api/KPI/UpdateRequiredEvaluateKPI', kpi);
        },
        deleteKPI: function (kpiId) {
            return $http['delete'](baseUrl + 'api/KPI/DeleteKPI', { params: { kpiId: kpiId} });
        },
        getUserPermissionOnKPI: function () {
            return $http.get(baseUrl + 'api/KPI/GetUserPermissionOnKPI');
        }
    };
} ]);