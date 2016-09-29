(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('disciplineModule').controller('OtherViolationsController', otherViolationsController);
    otherViolationsController.$inject = ['$modalInstance', 'DisciplineSummaryModel', 'utilitiesFactory', 'employee', 'startDate', 'endDate'];

    function otherViolationsController($modalInstance, DisciplineSummaryModel, utilitiesFactory, employee, startDate, endDate) {
        var vm = this;

        vm.employee = employee;
        vm.endDate = endDate;
        vm.otherViolations = [];
        vm.startDate = startDate;

        vm.cancel = cancel;

        utilitiesFactory.getDateFormat(vm);
        initialize();

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function getOtherViolations(employeeId, startDate, endDate) {
            DisciplineSummaryModel.getOtherViolations(employeeId, startDate, endDate).then(function () {
                vm.otherViolations = DisciplineSummaryModel.otherViolations;
            });
        }

        function initialize() {
            if (vm.employee) {
                getOtherViolations(vm.employee.Employee.EmployeeId, vm.startDate, vm.endDate);
            }
        }
    }
})(angular);