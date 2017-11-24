(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('disciplineModule').factory('disciplineSummaryFactory', disciplineSummaryFactory);
    disciplineSummaryFactory.$inject = ['$http', 'baseUrl'];

    function disciplineSummaryFactory($http, baseUrl) {
        return {
            getDisciplineSummary: getDisciplineSummary,
            getDisciplineSummaryForOtherSystem: getDisciplineSummaryForOtherSystem
        };

        function getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId, isShowAll) {
            return $http.get(baseUrl + 'api/Discipline/GetDisciplineSummary', {
                params: {
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

        function getDisciplineSummaryForOtherSystem(startDate, endDate, departmentId, isLocal, employeeId, reasonId) {
            return $http.get(baseUrl + 'api/Discipline/GetDisciplineSummaryForOtherSystem', {
                params: {
                    startDate: startDate,
                    endDate: endDate,
                    departmentId: departmentId,
                    isLocal: isLocal,
                    employeeId: employeeId,
                    reasonId: reasonId
                }
            });
        }
    }
})(angular);