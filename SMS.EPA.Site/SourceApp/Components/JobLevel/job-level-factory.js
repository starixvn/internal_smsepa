(function (angular) {
    'use strict';

    angular.module('jobLevelModule').factory('jobLevelFactory', jobLevelFactory);
    jobLevelFactory.$inject = ['$http', 'baseUrl'];

    function jobLevelFactory($http, baseUrl) {
        return {
            getUserPermission: getUserPermission,
            createJobLevel: createJobLevel,
            editJobLevel: editJobLevel,
            deleteJobLevel: deleteJobLevel
        };

        function getUserPermission() {
            return $http.get(baseUrl + 'api/JobLevel/GetUserPermission');
        }

        function createJobLevel(jobLevel) {
            return $http.post(baseUrl + 'api/JobLevel/Create', jobLevel);
        }

        function editJobLevel(jobLevel) {
            return $http.post(baseUrl + 'api/JobLevel/Edit', jobLevel);
        }

        function deleteJobLevel(jobLevelId) {
            return $http['delete'](baseUrl + 'api/JobLevel/Delete', { params: { jobLevelId: jobLevelId } });
        }
    }
})(angular);