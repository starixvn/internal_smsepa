(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('ChangePasswordEmployeeController', changePasswordEmployeeController);
    changePasswordEmployeeController.$inject = ['$modalInstance', 'EmployeeModel'];

    function changePasswordEmployeeController($modalInstance, EmployeeModel) {
        var vm = this;

        vm.password = {
            oldPassword: '',
            newPassword: '',
            confirmPassword: ''
        };

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function save(password) {
            if (EmployeeModel.validatePassword(password)) {
                EmployeeModel.changePassword(password, $modalInstance);
            }
        }
    }
})(angular);