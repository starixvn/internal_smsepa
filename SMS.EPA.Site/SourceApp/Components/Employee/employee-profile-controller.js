(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('EmployeeProfileController', employeeProfileController);
    employeeProfileController.$inject = ['$window', 'EmployeeModel'];

    function employeeProfileController($window, EmployeeModel) {
        var vm = this;

        vm.changePassword = changePassword;
        vm.logout = logout;

        function changePassword() {
            EmployeeModel.openPopupChangePassword(vm).then()['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function logout() {
            $window.location.href = "Auth/Logout";
        }
    }
})(angular);