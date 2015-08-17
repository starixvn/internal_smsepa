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

epaModule.directive('ngDecimal', [function () {
    return {
        require: 'ngModel',
        scope: {
            fraction: "@"
        },
        link: function (scope, element, attr, ctrl) {
            var fraction = parseInt(scope.fraction) || 2;

            ctrl.$parsers.push(function (inputValue) {
                if (inputValue) {
                    var digits = inputValue.replace(/[^\d+\.?\d+$]/, '');

                    if ((parseFloat(digits) * Math.pow(10, fraction)) % 1 !== 0) {
                        digits = parseFloat(digits).toFixed(fraction).toString();
                    }

                    if (digits !== inputValue) {
                        ctrl.$setViewValue(digits);
                        ctrl.$render();
                    }

                    return digits;
                }

                return '';
            });
        }
    };
}]);