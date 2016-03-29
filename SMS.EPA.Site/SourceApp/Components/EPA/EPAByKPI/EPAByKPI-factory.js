'use strict';

angular
    .module('epaModule')
    .factory('epaByKPIFactory', epaByKPIFactory);

epaByKPIFactory.$inject = ['$http', 'baseUrl'];

function epaByKPIFactory($http, baseUrl) {
    return {
        getTargetTemplateToView: getTargetTemplateToView,
        getKPIToView: getKPIToView,
        getEvaluatorsByTargetTemplate: getEvaluatorsByTargetTemplate,
        getEmployeeEPAToView: getEmployeeEPAToView
    };

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
}