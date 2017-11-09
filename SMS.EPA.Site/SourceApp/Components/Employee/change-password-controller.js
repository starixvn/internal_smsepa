(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('ChangePasswordEmployeeController', changePasswordEmployeeController);
    changePasswordEmployeeController.$inject = ['$uibModalInstance', 'EmployeeModel'];

    function changePasswordEmployeeController($uibModalInstance, EmployeeModel) {
        var vm = this;

        vm.password = {
            oldPassword: '',
            newPassword: '',
            confirmPassword: ''
        };

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save(password) {
            if (EmployeeModel.validatePassword(password)) {
                EmployeeModel.changePassword(password, $uibModalInstance);
            }
        }
    }
})(angular);