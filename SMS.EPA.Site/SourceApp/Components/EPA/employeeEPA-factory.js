'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getMemberByEvaluator: function (groupId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetMemberByEvaluator', { params: { groupId: groupId } });
        },
        getMemberByReviewer: function (groupId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetMemberByReviewer', { params: { groupId: groupId } });
        },
        getMemberByManagement: function (groupId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetMemberByManagement', { params: { groupId: groupId } });
        },
        getEPAByEmployee: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEmployee');
        },
        getEPAInfoByEmployee: function (epaId, actorUserId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEmployee', {
                params: {
                    epaId: epaId,
                    actorUserId: actorUserId
                }
            });
        },
        getEPAByEvaluator: function (groupId, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEvaluator', {
                params: {
                    groupId: groupId,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByEvaluator: function (epaId, actorUserId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEvaluator', {
                params: {
                    epaId: epaId,
                    actorUserId: actorUserId
                }
            });
        },
        getEPAByReviewer: function (groupId, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByReviewer', {
                params: {
                    groupId: groupId,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByReviewer: function (epaId, actorUserId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByReviewer', {
                params: {
                    epaId: epaId,
                    actorUserId: actorUserId
                }
            });
        },
        getEPAByManagement: function (groupId, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByManagement', {
                params: {
                    groupId: groupId,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByManagement: function (epaId, actorUserId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByManagement', {
                params: {
                    epaId: epaId,
                    actorUserId: actorUserId
                }
            });
        },
        getEPAInfoById: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoById', { params: { epaId: epaId } });
        },
        updateEPAScoreByEmployee: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEPAScoreByEmployee', model);
        },
        updateEPAScoreByEvaluator: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEPAScoreByEvaluator', model);
        },
        adjustScoreByReviewer: function (models) {
            return $http.put(baseUrl + 'api/EmployeeEPA/AdjustScoreByReviewer', models);
        },
        adjustScoreByManagement: function (models) {
            return $http.put(baseUrl + 'api/EmployeeEPA/AdjustScoreByManagement', models);
        },
        convertEPA: function () {
            return $http.put(baseUrl + 'api/EmployeeEPA/ConvertEPA');
        },
    };
}]);