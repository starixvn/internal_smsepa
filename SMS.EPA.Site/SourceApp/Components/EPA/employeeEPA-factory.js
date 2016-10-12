'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getUserPermissionOnManagement: function () {
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
        getEPAByManagement: function (groupId, isLocal, employeeIds, statusProcess) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByManagement', {
                params: {
                    groupId: groupId,
                    isLocal: isLocal,
                    employeeIds: employeeIds,
                    statusProcess: statusProcess
                }
            });
        },
        getEPAInfoByManagement: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByManagement', { params: { epaId: epaId } });
        },
        getEPAInfoById: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoById', { params: { epaId: epaId } });
        },
        adjustScoreByManagement: function (criteria) {
            return $http.put(baseUrl + 'api/EmployeeEPA/AdjustScoreByManagement', criteria);
        },
        convertEPA: function () {
            return $http.put(baseUrl + 'api/EmployeeEPA/ConvertEPA');
        },
        resetScore: function (epaId) {
            return $http.put(baseUrl + 'api/EmployeeEPA/ResetScore', { params: { epaId: epaId } });
        },
        getPeriodToUpdateFinalScore: function () {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetPeriodToUpdateFinalScore');
        },
        getEmployeeToUpdateFinalScore: function (periodId, groupId, isLocal, query) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEmployeeToUpdateFinalScore',
            {
                params: {
                    periodId: periodId,
                    groupId: groupId,
                    isLocal: isLocal,
                    query: query
                }
            });
        },
        getEPAToUpdateFinalScore: function (periodId, groupId, isLocal, employeeId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAToUpdateFinalScore',
            {
                params: {
                    periodId: periodId,
                    groupId: groupId,
                    isLocal: isLocal,
                    employeeId: employeeId
                }
            });
        },
        updateFinalScore: function (selectedEPAs) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateFinalScore', selectedEPAs);
        }
    };
}]);