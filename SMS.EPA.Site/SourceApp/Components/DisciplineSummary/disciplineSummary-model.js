(function (angular) {
    'use strict';

    angular.module('disciplineSummaryModule').factory('DisciplineSummaryModel', disciplineSummaryModel);
    disciplineSummaryModel.$inject = ['disciplineSummaryFactory', 'errorFactory'];

    function disciplineSummaryModel(disciplineSummaryFactory, errorFactory) {
        var employeeDisciplines = [];

        var model = {
            employeeDisciplines: employeeDisciplines,
            getDisciplineSummaryByPeriod: getDisciplineSummaryByPeriod
        };

        return model;

        function getDisciplineSummaryByPeriod(periodId, disciplineTypeId, departmentId, isLocal, employeeId) {
            return disciplineSummaryFactory.getDisciplineSummaryByPeriod(periodId, disciplineTypeId, departmentId, isLocal, employeeId)
            .success(function (employeeDisciplines) {
                model.employeeDisciplines = employeeDisciplines;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);