'use strict';
/*global epaModule*/

epaModule.factory('ratingByKPIFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getUserPermissionOnRatingByKPI: function(){
            return $http.get(baseUrl + 'api/EmployeeEPA/GetUserPermissionOnRatingByKPI');
        },
        getTargetTemplateByEvaluator: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetTargetTemplateByEvaluator');
        },
        getKPIByTargetTemplate: function (targetTemplateId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetKPIByTargetTemplate', { params: { targetTemplateId: targetTemplateId } });
        },
        getEmployeeEPAByKPI: function (targetTemplateId, kpiId, groupId, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEmployeeEPAByKPI', {
                params: {
                    targetTemplateId: targetTemplateId,
                    kpiId: kpiId,
                    groupId: groupId,
                    employeeIds: employeeIds
                }
            });
        },
        updateKPIScore: function (models) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateKPIScore', models);
        }
    };
}]);