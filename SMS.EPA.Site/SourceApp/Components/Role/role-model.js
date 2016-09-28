(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('roleModule').factory('RoleModel', roleModel);
    roleModel.$inject = ['roleFactory', 'errorFactory', 'notificationFactory'];

    function roleModel(roleFactory, errorFactory, notificationFactory) {
        var roleFunctions = [];

        var model = {
            assignFunctionToRole: assignFunctionToRole,
            getRoleFunctionByRoleId: getRoleFunctionByRoleId,
            roleFunctions: roleFunctions
        };

        return model;

        function assignFunctionToRole(roleFunction) {
            return roleFactory.assignFunctionToRole(roleFunction).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                } else {
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getRoleFunctionByRoleId(roleId) {
            return roleFactory.getRoleFunctionByRoleId(roleId).success(function (functions) {
                model.roleFunctions = functions;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);