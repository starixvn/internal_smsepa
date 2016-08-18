'use strict';

(function (angular) {
    angular.module('roleModule').factory('RoleModel', RoleModel);
    RoleModel.$inject = ['roleFactory', 'errorFactory', 'notificationFactory'];

    function RoleModel(roleFactory, errorFactory, notificationFactory) {
        var roleFunctions = [];
        var roleModel = {
            assignFunctionToRole: assignFunctionToRole,
            getRoleFunctionByRoleId: getRoleFunctionByRoleId,
            roleFunctions: roleFunctions
        };

        return roleModel;

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
                roleModel.roleFunctions = functions;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);