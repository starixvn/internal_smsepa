(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('employeeModule').controller('AssignRoleToUserController', assignRoleToUserController);
    assignRoleToUserController.$inject = ['$modalInstance', 'EmployeeModel', 'CommonModel', 'employee'];

    function assignRoleToUserController($modalInstance, EmployeeModel, CommonModel, employee) {
        var vm = this;

        vm.employee = employee;
        vm.systemRoles = [];

        vm.cancel = cancel;
        vm.checkRolePermission = checkRolePermission;
        vm.save = save;

        initialize();

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function checkRolePermission(roleId) {
            var isChecked = false;
            angular.forEach(vm.employee.Roles, function (role) {
                if (role.RoleId === roleId) {
                    isChecked = true;
                }
            });

            return isChecked;
        }

        function initialize() {
            CommonModel.getSystemRoles(true).then(function () {
                vm.systemRoles = CommonModel.systemRoles;
            });
        }

        function save(roleIds) {
            var roles = [];

            if (roleIds) {
                roles = EmployeeModel.createRoles(roleIds, vm.systemRoles);
            }

            var editedRole = {
                roles: roles,
                roleIds: roleIds
            };
            $modalInstance.close(editedRole);
        }
    }
})(angular);