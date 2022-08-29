(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('assessmentFactory', assessmentFactory);
    assessmentFactory.$inject = ['$http', 'baseUrl'];

    function assessmentFactory($http, baseUrl) {
        return {
            getPeriods,
            getOrgChart,
            getAssessments,
            edit,
            editStatus
        };

        function getPeriods() {
            return $http.get(baseUrl + 'api/JobLevelAssessment/GetPeriods');
        }

        function getOrgChart(periodId) {
            return $http.get(baseUrl + 'api/JobLevelAssessment/GetOrgChart', { params: { periodId } });
        }

        function getAssessments(params) {
            return $http.post(baseUrl + 'api/JobLevelAssessment/GetAssessments', params);
        }

        function edit(assessment) {
            return $http.put(baseUrl + 'api/JobLevelAssessment/Edit', assessment);
        }

        function editStatus(params) {
            return $http.put(baseUrl + 'api/JobLevelAssessment/EditStatus', params);
        }
    }
})(angular);