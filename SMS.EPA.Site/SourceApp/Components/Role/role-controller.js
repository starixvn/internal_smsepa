'use strict';

(function (angular) {
    angular.module('roleModule').controller('RoleController', RoleController);
    RoleController.$inject = ['RoleModel', 'CommonModel'];

    function RoleController(RoleModel, CommonModel) {
        var vm = this;

        vm.filter = {
            roleId: ''
        };
        vm.query = query;
        vm.roleFunctions = [];
        vm.save = save;
        vm.systemRoleOptions = [];

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