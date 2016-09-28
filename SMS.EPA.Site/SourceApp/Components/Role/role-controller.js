(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('roleModule').controller('RoleController', roleController);
    roleController.$inject = ['RoleModel', 'CommonModel'];

    function roleController(RoleModel, CommonModel) {
        var vm = this;

        vm.filter = {
            roleId: ''
        };
        vm.roleFunctions = [];
        vm.systemRoleOptions = [];

        vm.query = query;
        vm.save = save;

        initialize();

        function getFunctionsOfRole(roleId) {
            RoleModel.getRoleFunctionByRoleId(roleId).then(function () {
                vm.roleFunctions = RoleModel.roleFunctions;
                if (roleId === 1) {
                    vm.isSystemAdmin = true;
                }
                else {
                    vm.isSystemAdmin = false;
                }
            });
        }

        function initialize() {
            CommonModel.getSystemRoles(true, false).then(function () {
                vm.systemRoleOptions = CommonModel.systemRoles;

                if (vm.systemRoleOptions && vm.systemRoleOptions.length > 0) {
                    vm.filter.roleId = vm.systemRoleOptions[0].RoleId;
                    getFunctionsOfRole(vm.filter.roleId);
                }
            });
        }

        function query(filter) {
            getFunctionsOfRole(filter.roleId);
        }

        function save(roleFunction) {
            RoleModel.assignFunctionToRole(roleFunction);
        }
    }
})(angular);