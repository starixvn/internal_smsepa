(function (angular) {
    'use strict';

    angular.module('kpiGroupModule').factory('kpiGroupFactory', kpiGroupFactory);
    kpiGroupFactory.$inject = ['$http', 'baseUrl'];

    function kpiGroupFactory($http, baseUrl) {
        return {
            createKPIGroup: createKPIGroup,
            deleteKPIGroup: deleteKPIGroup,
            getKPIGroup: getKPIGroup,
            getKPIGroupById: getKPIGroupById,
            getUserPermissionOnKPIGroup: getUserPermissionOnKPIGroup,
            updateKPIGroup: updateKPIGroup,
            updateStatusKPIGroup: updateStatusKPIGroup
        };

        function createKPIGroup(kpiGroup) {
            return $http.post(baseUrl + 'api/KPIGroup/CreateKPIGroup', kpiGroup);
        }

        function deleteKPIGroup(kpiGroupId) {
            return $http['delete'](baseUrl + 'api/KPIGroup/DeleteKPIGroup', { params: { kpiGroupId: kpiGroupId } });
        }

        function getKPIGroup(isActive) {
            return $http.get(baseUrl + 'api/KPIGroup/GetKPIGroup', { params: { isActive: isActive } });
        }

        function getKPIGroupById(kpiGroupId) {
            return $http.get(baseUrl + 'api/KPIGroup/GetKPIGroupById', { params: { kpiGroupId: kpiGroupId } });
        }

        function getUserPermissionOnKPIGroup() {
            return $http.get(baseUrl + 'api/KPIGroup/GetUserPermissionOnKPIGroup');
        }

        function updateKPIGroup(kpiGroup) {
            return $http.put(baseUrl + 'api/KPIGroup/UpdateKPIGroup', kpiGroup);
        }

        function updateRequiredEvaluateKPI(kpi) {
            return $http.put(baseUrl + 'api/KPI/UpdateRequiredEvaluateKPI', kpi);
        }

        function updateStatusKPIGroup(kpiGroup) {
            return $http.put(baseUrl + 'api/KPIGroup/UpdateStatusKPIGroup', kpiGroup);
        }
    }
})(angular);