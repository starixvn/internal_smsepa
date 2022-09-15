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

        function getDisciplineSummary(startDate, endDate, teamIds, isLocal, employeeId, reasonId, isShowAll) {
            return $http.get(baseUrl + 'api/Discipline/GetDisciplineSummary', {
                params: { startDate, endDate, teamIds, isLocal, employeeId, reasonId, isShowAll }
            });
        }

        function getDisciplineSummaryForOtherSystem(startDate, endDate, teamIds, isLocal, employeeId, reasonId) {
            return $http.get(baseUrl + 'api/Discipline/GetDisciplineSummaryForOtherSystem', {
                params: { startDate, endDate, teamIds, isLocal, employeeId, reasonId }
            });
        }
    }
})(angular);