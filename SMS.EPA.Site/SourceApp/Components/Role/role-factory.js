'use strict';
/*global roleModule*/

roleModule.factory('roleFactory', ['$http', function ($http) {
    return {
        getRoleFunctionByRoleId: function (roleId) {
            return $http.get('/api/Role/GetRoleFunctionByRoleId', { params: { roleId: roleId} });
        },
        assignFunctionToRole: function (roleFunction) {
            return $http.put('/api/Role/AssignFunctionToRole', roleFunction);
        }
    };
} ]);