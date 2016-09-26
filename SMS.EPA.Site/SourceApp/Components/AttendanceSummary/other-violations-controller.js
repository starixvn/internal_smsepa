(function (angular) {
    'use strict';

    angular.module('attendanceModule').controller('OtherViolationsController', otherViolationsController);
    otherViolationsController.$inject = ['$modalInstance', 'AttendanceModel', 'utilitiesFactory', 'employee', 'startDate', 'endDate'];

    function otherViolationsController($modalInstance, AttendanceModel, utilitiesFactory, employee, startDate, endDate) {
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
        };

        function getOtherViolations(employeeId, startDate, endDate) {
            AttendanceModel.getOtherViolations(employeeId, startDate, endDate).then(function () {
                vm.otherViolations = AttendanceModel.otherViolations;
            });
        }

        function initialize() {
            if (vm.employee) {
                getOtherViolations(vm.employee.Employee.EmployeeId, vm.startDate, vm.endDate);
            }
        }
    }
})(angular);