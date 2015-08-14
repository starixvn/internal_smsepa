'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEPAOfCurrentMember: function (status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAOfCurrentMember', { params: { status: status } });
        },
        getEPAInformation: function (epaId, periodId, employeeId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInformation', {
                params: {
                    epaId: epaId,
                    periodId: periodId,
                    employeeId: employeeId
                }
            });
        },
        updateEmployeeEPA: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEmployeeEPA', model);
        }
    };
}]);