'use strict';
/*global epaModule*/

epaModule.factory('ratingByKPIFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getTargetTemplateByEvaluator: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetTargetTemplateByEvaluator');
        },
        getKPIByTargetTemplate: function (targetTemplateId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetKPIByTargetTemplate', { params: { targetTemplateId: targetTemplateId } });
        },
        getEmployeeEPAByKPI: function (targetTemplateId, kpiId, groupId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEmployeeEPAByKPI', {
                params: {
                    targetTemplateId: targetTemplateId,
                    kpiId: kpiId,
                    groupId: groupId
                }
            });
        }
    };
}]);