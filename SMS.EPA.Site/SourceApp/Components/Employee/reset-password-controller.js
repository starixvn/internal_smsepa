(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('ResetPasswordController', resetPasswordController);
    resetPasswordController.$inject = ['$uibModalInstance', 'employee', 'defaultPassword'];

    function resetPasswordController($uibModalInstance, employee, defaultPassword) {
        var vm = this;

        vm.defaultPassword = defaultPassword;
        vm.employee = employee;

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            $uibModalInstance.close();
        }
    }
})(angular);