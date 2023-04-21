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
            return $http.post(baseUrl + 'api/JobLevelStandardKPI/CreateJobLevelStandardKPI', kpi);
        }

        function deleteKPI(kpiId) {
            return $http['delete'](baseUrl + 'api/JobLevelStandardKPI/DeleteStatusJobLevelStandardKPI', { params: { kpiId: kpiId } });
        }

        function getJobLevelStandKPI(isActive) {
            return $http.get(baseUrl + 'api/JobLevelStandardKPI/GetJobLevelStandardKPI', {
                params: {
                    isActive: isActive
                }
            });
        }

        function getUserPermissionOnKPI() {
            return $http.get(baseUrl + 'api/JobLevelStandardKPI/GetUserPermissionOnKPI');
        }

        function updateKPI(kpi) {
            return $http.put(baseUrl + 'api/JobLevelStandardKPI/UpdateJobLevelStandardKPI', kpi);
        }

        function updateStatusKPI(kpi) {
            return $http.put(baseUrl + 'api/JobLevelStandardKPI/UpdateStatusJobLevelStandardKPI', kpi);
        }
    }
})(angular);