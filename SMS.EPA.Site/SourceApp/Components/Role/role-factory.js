'use strict';

(function (angular) {
    angular.module('roleModule').factory('roleFactory', roleFactory);
    roleFactory.$inject = ['$http', 'baseUrl'];

    function roleFactory($http, baseUrl) {
        return {
            assignFunctionToRole: assignFunctionToRole,
            getRoleFunctionByRoleId: getRoleFunctionByRoleId
        };

        function getRoleFunctionByRoleId(roleId) {
            return $http.get(baseUrl + 'api/Role/GetRoleFunctionByRoleId', { params: { roleId: roleId } });
        }

        function assignFunctionToRole(roleFunction) {
            return $http.put(baseUrl + 'api/Role/AssignFunctionToRole', roleFunction);
        }
    }
})(angular);