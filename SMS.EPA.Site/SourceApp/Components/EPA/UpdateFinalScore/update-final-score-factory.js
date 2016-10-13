(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('updateFinalScoreFactory', updateFinalScoreFactory);
    updateFinalScoreFactory.$inject = ['$http', 'baseUrl'];

    function updateFinalScoreFactory($http, baseUrl) {
        return {
            getEmployeeToUpdateFinalScore: getEmployeeToUpdateFinalScore,
            getEPAToUpdateFinalScore: getEPAToUpdateFinalScore,
            getPeriodToUpdateFinalScore: getPeriodToUpdateFinalScore,
            updateFinalScore: updateFinalScore
        };

        function getEmployeeToUpdateFinalScore(periodId, groupId, isLocal, query) {
            return $http.get(baseUrl + 'api/UpdateFinalScore/GetEmployeeToUpdateFinalScore', {
                params: {
                    periodId: periodId,
                    groupId: groupId,
                    isLocal: isLocal,
                    query: query
                }
            });
        }

        function getEPAToUpdateFinalScore(periodId, groupId, isLocal, employeeId) {
            return $http.get(baseUrl + 'api/UpdateFinalScore/GetEPAToUpdateFinalScore', {
                params: {
                    periodId: periodId,
                    groupId: groupId,
                    isLocal: isLocal,
                    employeeId: employeeId
                }
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