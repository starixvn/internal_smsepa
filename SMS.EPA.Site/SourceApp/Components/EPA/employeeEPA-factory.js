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
        getEPAOfCurrentMember: function (targetTypeId, status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAOfCurrentMember', {
                params: {
                    targetTypeId: targetTypeId,
                    status: status
                }
            });
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
        getEPAByEvaluator: function (fromDate, toDate, targetTypeId, groupId, employeeIds, status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEvaluator', {
                params: {
                    fromDate: fromDate,
                    toDate: toDate,
                    targetTypeId: targetTypeId,
                    groupId: groupId,
                    employeeIds: employeeIds,
                    status: status
                }
            });
        },
        updateEmployeeEPA: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEmployeeEPA', model);
        },
        updateStatusEPA: function (epa) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateStatusEPA', epa);
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