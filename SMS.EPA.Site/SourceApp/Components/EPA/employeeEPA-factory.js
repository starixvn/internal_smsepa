'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEPAOfCurrentMember: function (status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAOfCurrentMember', { params: { status: status } });
        },
        getEPAInformation: function (employeeId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInformation', { params: { employeeId: employeeId } });
        }
    };
}]);