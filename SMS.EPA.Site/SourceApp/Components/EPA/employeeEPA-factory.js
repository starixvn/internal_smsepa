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
        getEPAInfoByEmployee: function (epaId, employeeId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEmployee', {
                params: {
                    epaId: epaId,
                    employeeId: employeeId
                }
            });
        },
        getEPAByEvaluator: function (targetTypeId, groupId, employeeIds, status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEvaluator', {
                params: {
                    targetTypeId: targetTypeId,
                    groupId: groupId,
                    employeeIds: employeeIds,
                    status: status
                }
            });
        },
        getEPAInfoByEvaluator: function (epaId, periodId, employeeId, evaluatorId, isViewMode) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEvaluator', {
                params: {
                    epaId: epaId,
                    periodId: periodId,
                    employeeId: employeeId,
                    isViewMode: isViewMode,
                    evaluatorId: evaluatorId
                }
            });
        },
        getEPAByReviewer: function (fromDate, toDate, targetTypeId, groupId, employeeIds, status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByReviewer', {
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
        updateEPAScore: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEPAScore', model);
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