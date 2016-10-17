(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('ResetPasswordController', resetPasswordController);
    resetPasswordController.$inject = ['$modalInstance', 'employee', 'defaultPassword'];

    function resetPasswordController($modalInstance, employee, defaultPassword) {
        var vm = this;

        vm.defaultPassword = defaultPassword;
        vm.employee = employee;

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function save() {
            $modalInstance.close();
        }
    }
})(angular);