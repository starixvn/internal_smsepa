'use strict';
/*global roleModule*/

roleModule.controller('RoleController',
['RoleModel', 'CommonModel',
function (RoleModel, CommonModel) {
    var vm = this;

    vm.systemRoleOptions = CommonModel.systemRoles;
    vm.roleFunctions = RoleModel.roleFunctions;

    vm.filter = {
        roleId: ''
    };

    /*
    ** Get system roles.
    */
    CommonModel.getSystemRoles(true, false).then(function () {
        vm.systemRoleOptions = CommonModel.systemRoles;

        if (vm.systemRoleOptions && vm.systemRoleOptions.length > 0) {
            vm.filter.roleId = vm.systemRoleOptions[0].RoleId;
            getFunctionsOfRole(vm.filter.roleId);
        }
    });

    /*
    ** Get functions of role.
    */
    var getFunctionsOfRole = function (roleId) {
        RoleModel.getRoleFunctionByRoleId(roleId).then(function () {
            vm.roleFunctions = RoleModel.roleFunctions;
            if (roleId === 1) {
                vm.isSystemAdmin = true;
            }
            else {
                vm.isSystemAdmin = false;
            }
        });
    };

    /*
    ** Query.
    */
    vm.query = function (filter) {
        getFunctionsOfRole(filter.roleId);
    };

    /*
    ** Assign function to role.
    */
    vm.save = function (roleFunction) {
        RoleModel.assignFunctionToRole(roleFunction);
    };
} ]);