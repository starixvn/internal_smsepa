(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('ratingByKPIFactory', ratingByKPIFactory);
    ratingByKPIFactory.$inject = ['$http', 'baseUrl'];

    function ratingByKPIFactory($http, baseUrl) {
        return {
            getEmployeeEPAByKPI: getEmployeeEPAByKPI,
            getKPIByTargetTemplate: getKPIByTargetTemplate,
            getPeriodByEvaluator: getPeriodByEvaluator,
            getTargetTemplateByPeriod: getTargetTemplateByPeriod,
            getUserPermissionOnRatingByKPI: getUserPermissionOnRatingByKPI,
            updateKPIScore: updateKPIScore
        };

        function getEmployeeEPAByKPI(periodId, targetTemplateId, kpiId, groupId, employeeIds) {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetEmployeeEPAByKPI', {
                params: {
                    periodId: periodId,
                    targetTemplateId: targetTemplateId,
                    kpiId: kpiId,
                    groupId: groupId,
                    employeeIds: employeeIds
                }
            });
        }

        function getKPIByTargetTemplate(periodId, targetTemplateId) {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetKPIByTargetTemplate', {
                params: {
                    periodId: periodId,
                    targetTemplateId: targetTemplateId
                }
            });
        }

        function getPeriodByEvaluator() {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetPeriodByEvaluator');
        }

        function getTargetTemplateByPeriod(periodId) {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetTargetTemplateByEvaluator', { params: { periodId: periodId } });
        }

        function getUserPermissionOnRatingByKPI() {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetUserPermissionOnRatingByKPI');
        }

        function updateKPIScore(models) {
            return $http.put(baseUrl + 'api/EvaluatorByKPI/UpdateKPIScore', models);
        }
    }
})(angular);