(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('epaByKPIFactory', epaByKPIFactory);
    epaByKPIFactory.$inject = ['$http', 'baseUrl'];

    function epaByKPIFactory($http, baseUrl) {
        return {
            getPeriodToView: getPeriodToView,
            getTargetTemplateToView: getTargetTemplateToView,
            getKPIToView: getKPIToView,
            getEvaluatorsByTargetTemplate: getEvaluatorsByTargetTemplate,
            getEmployeeEPAToView: getEmployeeEPAToView
        };

        function getEvaluatorsByTargetTemplate(periodId, targetTemplateId) {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetEvaluatorsByTargetTemplate', {
                params: {
                    periodId: periodId,
                    targetTemplateId: targetTemplateId
                }
            });
        }

        function getEmployeeEPAToView(periodId, targetTemplateId, kpiId, evaluatorId) {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetEmployeeEPAToView', {
                params: {
                    periodId: periodId,
                    targetTemplateId: targetTemplateId,
                    kpiId: kpiId,
                    evaluatorId: evaluatorId
                }
            });
        }

        function getKPIToView(periodId, targetTemplateId, evaluatorId) {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetKPIToView', {
                params: {
                    periodId: periodId,
                    targetTemplateId: targetTemplateId,
                    evaluatorId: evaluatorId
                }
            });
        }

        function getPeriodToView() {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetPeriodToView');
        }

        function getTargetTemplateToView(periodId) {
            return $http.get(baseUrl + 'api/EvaluatorByKPI/GetTargetTemplateToView',
            {
                params: {
                    periodId: periodId
                }
            });
        }
    }
})(angular);