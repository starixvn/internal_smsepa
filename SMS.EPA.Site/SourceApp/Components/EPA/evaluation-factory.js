(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('evaluationFactory', evaluationFactory);
    evaluationFactory.$inject = ['$http', 'baseUrl'];

    function evaluationFactory($http, baseUrl) {
        return {
            convertEPA: convertEPA,
            getEPAInfoById: getEPAInfoById,
            resetScore: resetScore
        };

        function convertEPA() {
            return $http.put(baseUrl + 'api/Evaluation/ConvertEPA');
        }

        function getEPAInfoById(epaId) {
            return $http.get(baseUrl + 'api/Evaluation/GetEPAInfoById', { params: { epaId: epaId } });
        }

        function resetScore(epaId) {
            var criteria = {
              EPAId: epaId
            };

            return $http.put(baseUrl + 'api/Evaluation/ResetScore', criteria);
        }
    }
})(angular);