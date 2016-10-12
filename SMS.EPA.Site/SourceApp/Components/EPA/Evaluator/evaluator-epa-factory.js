(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('evaluatorEPAFactory', evaluatorEPAFactory);
    evaluatorEPAFactory.$inject = ['$http', 'baseUrl'];

    function evaluatorEPAFactory($http, baseUrl) {
        return {
            getEPAByEvaluator: getEPAByEvaluator,
            getEPAInfoByEvaluator: getEPAInfoByEvaluator,
            getGroupByEvaluator: getGroupByEvaluator,
            getMemberByEvaluator: getMemberByEvaluator,
            getUserPermissionOnEvaluator: getUserPermissionOnEvaluator,
            updateEPAScoreByEvaluator: updateEPAScoreByEvaluator
        };

        function getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds, statusProcess) {
            return $http.get(baseUrl + 'api/EvaluatorEPA/GetEPAByEvaluator', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    isMonthly: isMonthly,
                    employeeIds: employeeIds,
                    statusProcess: statusProcess
                }
            });
        }

        function getEPAInfoByEvaluator(epaId, evaluatorId) {
            return $http.get(baseUrl + 'api/EvaluatorEPA/GetEPAInfoByEvaluator', {
                params: {
                    epaId: epaId,
                    evaluatorId: evaluatorId
                }
            });
        }

        function getGroupByEvaluator() {
            return $http.get(baseUrl + 'api/EvaluatorEPA/GetGroupByEvaluator');
        }

        function getMemberByEvaluator(groupId, isLocal, isMonthly) {
            return $http.get(baseUrl + 'api/EvaluatorEPA/GetMemberByEvaluator', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    isMonthly: isMonthly
                }
            });
        }

        function getUserPermissionOnEvaluator() {
            return $http.get(baseUrl + 'api/EvaluatorEPA/GetUserPermissionOnEvaluator');
        }

        function updateEPAScoreByEvaluator(criteria) {
            return $http.put(baseUrl + 'api/EvaluatorEPA/UpdateEPAScoreByEvaluator', criteria);
        }
    }
})(angular);