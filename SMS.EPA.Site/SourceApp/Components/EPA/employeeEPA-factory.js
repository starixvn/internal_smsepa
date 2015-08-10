'use strict';
/*global epaModule*/

epaModule.factory('employeeEPAFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getEPAOfCurrentMember: function (status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAOfCurrentMember', { params: { status: status } });
        }
    };
}]);