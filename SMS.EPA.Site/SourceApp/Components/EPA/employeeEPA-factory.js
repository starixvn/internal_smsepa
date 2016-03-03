'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getUserPermissionOnEmployee: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetUserPermissionOnEmployee');
        },
        getEPAByEmployee: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEmployee');
        },
        getEPAInfoByEmployee: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEmployee', { params: { epaId: epaId } });
        },
        getUserPermissionOnEvaluator: function() {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetUserPermissionOnEvaluator');
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
        getEPAByEvaluator: function (groupId, isLocal, isMonthly, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEvaluator', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    isMonthly: isMonthly,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByEvaluator: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEvaluator', { params: { epaId: epaId } });
        },
        getUserPermissionOnReviewer: function(){
            return $http.get(baseUrl + 'api/EmployeeEPA/GetUserPermissionOnReviewer');
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
        getEPAByReviewer: function (groupId, isLocal, isMonthly, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByReviewer', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    isMonthy: isMonthly,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByReviewer: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByReviewer', { params: { epaId: epaId } });
        },
        getUserPermissionOnManagement: function(){
            return $http.get(baseUrl + 'api/EmployeeEPA/GetUserPermissionOnManagement');
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
        getEPAByManagement: function (groupId, isLocal, isMonthly, employeeIds) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByManagement', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    isMonthly: isMonthly,
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