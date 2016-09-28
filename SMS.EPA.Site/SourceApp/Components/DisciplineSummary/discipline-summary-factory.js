(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('disciplineModule').factory('disciplineSummaryFactory', disciplineSummaryFactory);
    disciplineSummaryFactory.$inject = ['$http', 'baseUrl'];

    function disciplineSummaryFactory($http, baseUrl) {
        return {
            getDisciplineDetail: getDisciplineDetail,
            getDisciplineSummary: getDisciplineSummary,
            getDisciplineSummaryByPeriod: getDisciplineSummaryByPeriod,
            getOtherViolations: getOtherViolations,
            getUserPermissionOnDisciplineSummary: getUserPermissionOnDisciplineSummary,
            getDisciplineType: getDisciplineType
        };

        function getDisciplineDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored) {
            return $http.get(baseUrl + 'api/DisciplineSummary/GetDisciplineDetail', {
                params: {
                    startDate: startDate,
                    endDate: endDate,
                    employeeId: employeeId,
                    departmentId: departmentId,
                    reasonId: reasonId,
                    isLocal: isLocal,
                    hasIgnored: hasIgnored
                }
            });
        }

        function getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId, isShowAll) {
            return $http.get(baseUrl + 'api/DisciplineSummary/GetDisciplineSummary', {
                params:
                    {
                        startDate: startDate,
                        endDate: endDate,
                        departmentId: departmentId,
                        isLocal: isLocal,
                        employeeId: employeeId,
                        reasonId: reasonId,
                        isShowAll: isShowAll
                    }
            });
        }

        function getDisciplineSummaryByPeriod(periodId, disciplineTypeId, departmentId, isLocal, employeeId) {
            return $http.get(baseUrl + 'api/DisciplineSummary/GetDisciplineSummaryByPeriod', {
                params: {
                    periodId: periodId,
                    disciplineTypeId: disciplineTypeId,
                    departmentId: departmentId,
                    isLocal: isLocal,
                    employeeId: employeeId
                }
            });
        }

        function getOtherViolations(employeeId, startDate, endDate, workDate) {
            return $http.get(baseUrl + 'api/DisciplineSummary/GetOtherViolations', {
                params: {
                    employeeId: employeeId,
                    startDate: startDate,
                    endDate: endDate,
                    workDate: workDate
                }
            });
        }

        function getUserPermissionOnDisciplineSummary() {
            return $http.get(baseUrl + 'api/DisciplineSummary/GetUserPermissionOnDisciplineSummary');
        }

        function getDisciplineType(includeOtherViolate, isActive) {
            return $http.get(baseUrl + 'api/DisciplineSummary/GetDisciplineType', {
                params: {
                    includeOtherViolate: includeOtherViolate,
                    isActive: isActive
                }
            });
        }
    }
})(angular);