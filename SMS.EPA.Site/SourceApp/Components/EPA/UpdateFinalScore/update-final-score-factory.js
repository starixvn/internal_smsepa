(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('updateFinalScoreFactory', updateFinalScoreFactory);
    updateFinalScoreFactory.$inject = ['$http', 'baseUrl'];

    function updateFinalScoreFactory($http, baseUrl) {
        return {
            getEmployeeToUpdateFinalScore,
            getEPAToUpdateFinalScore,
            getPeriodToUpdateFinalScore,
            updateFinalScore
        };

        function getEmployeeToUpdateFinalScore(periodId, teamIds, isLocal, query) {
            return $http.get(baseUrl + 'api/UpdateFinalScore/GetEmployeeToUpdateFinalScore', {
                params: { periodId, teamIds, isLocal, query }
            });
        }

        function getEPAToUpdateFinalScore(periodId, teamIds, isLocal, employeeId) {
            return $http.get(baseUrl + 'api/UpdateFinalScore/GetEPAToUpdateFinalScore', {
                params: { periodId, teamIds, isLocal, employeeId }
            });
        }

        function getPeriodToUpdateFinalScore() {
            return $http.get(baseUrl + 'api/UpdateFinalScore/GetPeriodToUpdateFinalScore');
        }

        function updateFinalScore(epaInfos) {
            return $http.put(baseUrl + 'api/UpdateFinalScore/UpdateFinalScore', epaInfos);
        }
    }
})(angular);