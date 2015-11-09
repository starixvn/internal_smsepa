'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEPAByEmployee: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEmployee');
        },
        getEPAInfoByEmployee: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEmployee', { params: { epaId: epaId } });
        },
        getGroupByEvaluator: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetGroupByEvaluator');
        },
        getMemberByEvaluator: function (groupId, isLocal) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetMemberByEvaluator', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal
                }
            });
        },
        getEPAByEvaluator: function (groupId, isLocal, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEvaluator', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByEvaluator: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEvaluator', { params: { epaId: epaId } });
        },
        getGroupByReviewer: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetGroupByReviewer');
        },
        getMemberByReviewer: function (groupId, isLocal) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetMemberByReviewer', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal
                }
            });
        },
        getEPAByReviewer: function (groupId, isLocal, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByReviewer', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByReviewer: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByReviewer', { params: { epaId: epaId } });
        },
        getGroupByManagement: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetGroupByManagement');
        },
        getMemberByManagement: function (groupId, isLocal) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetMemberByManagement', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal
                }
            });
        },
        getEPAByManagement: function (groupId, isLocal, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByManagement', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
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