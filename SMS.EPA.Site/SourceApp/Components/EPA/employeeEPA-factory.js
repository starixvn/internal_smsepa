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
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAOfCurrentMember');
        },
        getEPAInfoByEmployee: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEmployee', { params: { epaId: epaId } });
        },
        getEPAByEvaluator: function (groupId, employeeIds, status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByEvaluator', {
                params: {
                    groupId: groupId,
                    employeeIds: employeeIds
                }
            });
        },
        getEPAInfoByEvaluator: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByEvaluator', { params: { epaId: epaId } });
        },
        getEPAByReviewer: function (targetTypeId, groupId, employeeIds, status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByReviewer', {
                params: {
                    targetTypeId: targetTypeId,
                    groupId: groupId,
                    employeeIds: employeeIds,
                    status: status
                }
            });
        },
        getEPAInfoByReviewer: function (epaId, employeeId, evaluatorId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByReviewer', {
                params: {
                    epaId: epaId,
                    employeeId: employeeId,
                    evaluatorId: evaluatorId
                }
            });
        },
        getEPAByManagement: function (groupId, employeeIds, status) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAByManagement', {
                params: {
                    groupId: groupId,
                    employeeIds: employeeIds,
                    status: status
                }
            });
        },
        getEPAInfoByManagement: function (epaId) {
            return $http.get(baseUrl + 'api/EmployeeEPA/GetEPAInfoByManagement', { params: { epaId: epaId } });
        },
        updateEPAScore: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEPAScore', model);
        },
        updateScoreByReviewerOrManagement: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateScoreByReviewerOrManagement', model);
        },
        updateEPAScoreByEvaluator: function (model) {
            return $http.put(baseUrl + 'api/EmployeeEPA/UpdateEPAScoreByEvaluator', model);
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