'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getMember: function (processId, groupId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetMember', {
                params: {
                    processId: processId,
                    groupId: groupId
                }
            });
        },
        getEPAOfCurrentMember: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAOfCurrentMember');
        },
        getEPAInfoByEmployee: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEmployee', { params: { epaId: epaId } });
        },
        getEPAByEvaluator: function (groupId, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEvaluator', {
                params: {
                    groupId: groupId,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByEvaluator: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEvaluator', { params: { epaId: epaId } });
        },
        getEPAByReviewer: function (groupId, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByReviewer', {
                params: {
                    groupId: groupId,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByReviewer: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByReviewer', { params: { epaId: epaId } });
        },
        getEPAByManagement: function (groupId, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByManagement', {
                params: {
                    groupId: groupId,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByManagement: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByManagement', { params: { epaId: epaId } });
        },
        getEPAInfoById: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoById', { params: { epaId: epaId } });
        },
        updateEPAScore: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEPAScore', model);
        },
        updateScoreByReviewerOrManagement: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateScoreByReviewerOrManagement', model);
        },
        updateEPAScoreByEvaluator: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEPAScoreByEvaluator', model);
        },
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