(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('JobLevelStandardKPIFactory', kpiFactory);
    kpiFactory.$inject = ['$http', 'baseUrl'];

    function kpiFactory($http, baseUrl) {
        return {
            createKPI: createKPI,
            deleteKPI: deleteKPI,
            getJobLevelStandKPI: getJobLevelStandKPI,
            getUserPermissionOnKPI: getUserPermissionOnKPI,
            updateKPI: updateKPI,
            updateStatusKPI: updateStatusKPI
        };

        function createKPI(kpi) {
            return $http.post(baseUrl + 'api/JobLevelStandardKPI/Create', kpi);
        }

        function deleteKPI(kpiId) {
            return $http['delete'](baseUrl + 'api/JobLevelStandardKPI/Delete', { params: { kpiId: kpiId } });
        }

        function getJobLevelStandKPI(isActive) {
            return $http.get(baseUrl + 'api/JobLevelStandardKPI/GetKPI', {
                params: {
                    isActive: isActive
                }
            });
        }

        function getUserPermissionOnKPI() {
            return $http.get(baseUrl + 'api/JobLevelStandardKPI/GetUserPermission');
        }

        function updateKPI(kpi) {
            return $http.put(baseUrl + 'api/JobLevelStandardKPI/Update', kpi);
        }

        function updateStatusKPI(kpi) {
            return $http.put(baseUrl + 'api/JobLevelStandardKPI/UpdateStatus', kpi);
        }
    }
})(angular);