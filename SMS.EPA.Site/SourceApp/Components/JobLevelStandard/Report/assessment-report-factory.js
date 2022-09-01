(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('assessmentReportFactory', assessmentReportFactory);
    assessmentReportFactory.$inject = ['$http', 'baseUrl'];

    function assessmentReportFactory($http, baseUrl) {
        return {
            getPeriods,
            getOrgChart,
            getAssessments
        };

        function getPeriods() {
            return $http.get(baseUrl + 'api/JobLevelAssessmentReport/GetPeriods');
        }

        function getOrgChart(periodId) {
            return $http.get(baseUrl + 'api/JobLevelAssessmentReport/GetOrgChart', { params: { periodId } });
        }

        function getAssessments(params) {
            return $http.post(baseUrl + 'api/JobLevelAssessmentReport/GetAssessments', params);
        }
    }
})(angular);