'use strict';
(function (angular) {
    function epaByKPIFactory($http, baseUrl) {
        function getEvaluatorsByTargetTemplate(targetTemplateId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/getEvaluatorsByTargetTemplate', {
                params: {
                    targetTemplateId: targetTemplateId
                }
            });
        }

        function getEmployeeEPAToView(targetTemplateId, kpiId, evaluatorId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEmployeeEPAToView', {
                params: {
                    targetTemplateId: targetTemplateId,
                    kpiId: kpiId,
                    evaluatorId: evaluatorId
                }
            });
        }

        function getKPIToView(targetTemplateId, evaluatorId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/getKPIToView', {
                params: {
                    targetTemplateId: targetTemplateId,
                    evaluatorId: evaluatorId
                }
            });
        }

        function getTargetTemplateToView() {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetTargetTemplateToView');
        }

        return {
            getTargetTemplateToView: getTargetTemplateToView,
            getKPIToView: getKPIToView,
            getEvaluatorsByTargetTemplate: getEvaluatorsByTargetTemplate,
            getEmployeeEPAToView: getEmployeeEPAToView
        };
    }

    epaByKPIFactory.$inject = ['$http', 'baseUrl'];

    angular
        .module('epaModule')
        .factory('epaByKPIFactory', epaByKPIFactory);
})(angular);
