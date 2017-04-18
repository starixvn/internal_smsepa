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

        function getEmployeeToUpdateFinalScore(periodId, departmentId, isLocal, query) {
            return $http.get(baseUrl + 'api/UpdateFinalScore/GetEmployeeToUpdateFinalScore', {
                params: {
                    periodId: periodId,
                    departmentId: departmentId,
                    isLocal: isLocal,
                    query: query
                }
            });
        }

        function getEPAToUpdateFinalScore(periodId, departmentId, isLocal, employeeId) {
            return $http.get(baseUrl + 'api/UpdateFinalScore/GetEPAToUpdateFinalScore', {
                params: {
                    periodId: periodId,
                    departmentId: departmentId,
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