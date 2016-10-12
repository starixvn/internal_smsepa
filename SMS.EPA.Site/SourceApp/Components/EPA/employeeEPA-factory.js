'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEPAInfoById: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoById', { params: { epaId: epaId } });
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