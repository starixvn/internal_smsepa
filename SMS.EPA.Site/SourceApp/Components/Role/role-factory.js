'use strict';
/*global roleModule*/

roleModule.factory('roleFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getRoleFunctionByRoleId: function (roleId) {
            return $http.get(baseUrl + 'api/Role/GetRoleFunctionByRoleId', { params: { roleId: roleId} });
        },
        assignFunctionToRole: function (roleFunction) {
            return $http.put(baseUrl + 'api/Role/AssignFunctionToRole', roleFunction);
        }
    };
} ]);