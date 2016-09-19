(function (angular) {
    'use strict';

    angular.module('disciplineSummaryModule').factory('disciplineSummaryFactory', disciplineSummaryFactory);
    disciplineSummaryFactory.$inject = ['$http', 'baseUrl'];

    function disciplineSummaryFactory($http, baseUrl) {
        return {
            getDisciplineSummaryByPeriod: getDisciplineSummaryByPeriod
        };

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
    }
})(angular);