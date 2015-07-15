'use strict';
/*global roleModule*/

roleModule.factory("RoleModel",
['roleFactory', 'errorFactory', 'notificationFactory',
function (roleFactory, errorFactory, notificationFactory) {
    var roleModel = {};

    roleModel.roleFunctions = [];

    /*
    ** Get functions of role by role ID.
    */
    roleModel.getRoleFunctionByRoleId = function (roleId) {
        return roleFactory.getRoleFunctionByRoleId(roleId).success(function (functions) {
            roleModel.roleFunctions = functions;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /*
    ** Assign function to role.
    */
    roleModel.assignFunctionToRole = function (roleFunction) {
        return roleFactory.assignFunctionToRole(roleFunction).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdateSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return roleModel;
} ]);