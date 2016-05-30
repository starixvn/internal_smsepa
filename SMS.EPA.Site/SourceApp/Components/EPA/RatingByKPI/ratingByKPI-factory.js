'use strict';
/*global epaModule*/

epaModule.factory('ratingByKPIFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getUserPermissionOnRatingByKPI: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetUserPermissionOnRatingByKPI');
        },
        getPeriodByEvaluator: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetPeriodByEvaluator');
        },
        getTargetTemplateByPeriod: function (periodId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetTargetTemplateByEvaluator', { params: { periodId: periodId } });
        },
        getKPIByTargetTemplate: function (periodId, targetTemplateId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetKPIByTargetTemplate', {
                params: {
                    periodId: periodId,
                    targetTemplateId: targetTemplateId
                }
            });
        },
        getEmployeeEPAByKPI: function (periodId, targetTemplateId, kpiId, groupId, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEmployeeEPAByKPI',
            {
                params: {
                    periodId: periodId,
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