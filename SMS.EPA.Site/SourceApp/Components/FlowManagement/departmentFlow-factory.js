(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').factory('deptFlowFactory', deptFlowFactory);
    deptFlowFactory.$inject = ['$http', 'baseUrl'];

    function deptFlowFactory($http, baseUrl) {
        return {
            createDepartmentFlow: function (model) {
                return $http.post(baseUrl + 'api/DepartmentFlow/CreateDepartmentFlow', model);
            },
            deleteDepartmentFlow: function (departmentId, flowTemplateId) {
                return $http['delete'](baseUrl + 'api/DepartmentFlow/DeleteDepartmentFlow', {
                    params: {
                        departmentId: departmentId,
                        flowTemplateId: flowTemplateId
                    }
                });
            },
            getDepartmentFlow: function (flowTemplateId) {
                return $http.get(baseUrl + 'api/DepartmentFlow/GetDepartmentFlowByFlowTemplateId', {
                    params: {
                        flowTemplateId: flowTemplateId
                    }
                });
            },
            getDepartmentFlowById: function (departmentId, flowTemplateId) {
                return $http.get(baseUrl + 'api/DepartmentFlow/GetDepartmentFlowById', {
                    params: {
                        departmentId: departmentId,
                        flowTemplateId: flowTemplateId
                    }
                });
            },
            getDepartmentHasSetting: function (flowTemplateId) {
                return $http.get(baseUrl + 'api/DepartmentFlow/GetDepartmentHasSetting', { params: { flowTemplateId: flowTemplateId } });
            }
        };
    }
})(angular);