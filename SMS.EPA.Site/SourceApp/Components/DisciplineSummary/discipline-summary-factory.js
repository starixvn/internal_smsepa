(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('disciplineModule').factory('disciplineSummaryFactory', disciplineSummaryFactory);
    disciplineSummaryFactory.$inject = ['$http', 'baseUrl'];

    function disciplineSummaryFactory($http, baseUrl) {
        return {
            getDisciplineSummary: getDisciplineSummary,
            getDisciplineSummaryForOtherSystem: getDisciplineSummaryForOtherSystem,
            getDisciplineType: getDisciplineType
        };

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

        function getDisciplineSummaryForOtherSystem(periodId, disciplineTypeId, departmentId, isLocal, employeeId) {
            return $http.get(baseUrl + 'api/DisciplineSummary/GetDisciplineSummaryForOtherSystem', {
                params: {
                    periodId: periodId,
                    disciplineTypeId: disciplineTypeId,
                    departmentId: departmentId,
                    isLocal: isLocal,
                    employeeId: employeeId
                }
            });
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