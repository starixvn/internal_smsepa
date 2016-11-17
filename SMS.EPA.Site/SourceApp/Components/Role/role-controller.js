(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('roleModule').controller('RoleController', roleController);
    roleController.$inject = ['RoleModel', 'CommonModel', 'lockScreenService'];

    function roleController(RoleModel, CommonModel, lockScreenService) {
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
            lockScreenService.start();

            RoleModel.getRoleFunctionByRoleId(roleId).then(function () {
                vm.roleFunctions = RoleModel.roleFunctions;
                if (roleId === 1) {
                    vm.isSystemAdmin = true;
                }
                else {
                    vm.isSystemAdmin = false;
                }

                lockScreenService.stop();
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