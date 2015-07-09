'use strict';
/*global kpiModule*/

kpiModule.factory('kpiFactory', ['$http', function ($http) {
    return {
        getKPI: function (kpiGroupId, isActive) {
            return $http.get('/api/KPI/GetKPI', { params: { kpiGroupId: kpiGroupId, isActive: isActive} });
        },
        insertKPI: function (kpi) {
            return $http.post('/api/KPI/InsertKPI', kpi);
        },
        updateKPI: function (kpi) {
            return $http.put('/api/KPI/UpdateKPI', kpi);
        },
        updateStatusKPI: function (kpi) {
            return $http.put('/api/KPI/UpdateStatusKPI', kpi);
        },
        deleteKPI: function (kpiId) {
            return $http['delete']('/api/KPI/DeleteKPI', { params: { kpiId: kpiId} });
        }
    };
} ]);