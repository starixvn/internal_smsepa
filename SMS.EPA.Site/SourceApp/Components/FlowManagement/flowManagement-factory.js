'use strict';
/*global flowMngModule*/

flowMngModule.factory('flowMngFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getFlowTemplate: function (targetTypeId, isActive) {
            return $http.get(baseUrl + 'api/FlowManagement/GetFlowTemplate', {
                params: {
                    targetTypeId: targetTypeId,
                    isActive: isActive
                }
            });
        },
        getFlowTemplateById: function(flowTemplateId){
            return $http.get(baseUrl + 'api/FlowManagement/GetFlowTemplateById', { params: { flowTemplateId: flowTemplateId } });
        },
        createFlowTemplate: function(flowTemplate){
            return $http.post(baseUrl + 'api/FlowManagement/CreateFlowTemplate', flowTemplate);
        },
        updateFlowTemplate: function(flowTemplate){
            return $http.put(baseUrl + 'api/FlowManagement/UpdateFlowTemplate', flowTemplate);
        },
        updateStatusFlowTemplate: function (flowTemplate) {
            return $http.put(baseUrl + 'api/FlowManagement/UpdateStatusFlowTemplate', flowTemplate);
        },
        deleteFlowTemplate: function (flowTemplateId) {
            return $http['delete'](baseUrl + 'api/FlowManagement/DeleteFlowTemplate', { params: { flowTemplateId: flowTemplateId } });
        },
        getDepartmentFlow: function (flowTemplateId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetDepartmentFlow', {
                params: {
                    flowTemplateId: flowTemplateId
                }
            });
        },
        getDepartmentFlowById: function (departmentId, flowTemplateId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetDepartmentFlowById', {
                params: {
                    departmentId: departmentId,
                    flowTemplateId: flowTemplateId
                }
            });
        },
        getPersonalFlow: function (flowTemplateId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetPersonalFlow', {
                params: {
                    flowTemplateId: flowTemplateId
                }
            });
        },
        getPersonalFlowById: function (employeeId, flowTemplateId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetPersonalFlowById', {
                params: {
                    employeeId: employeeId,
                    flowTemplateId: flowTemplateId
                }
            });
        },
        createDepartmentFlow: function (model) {
            return $http.post(baseUrl + 'api/FlowManagement/CreateDepartmentFlow', model);
        },
        createPersonalFlow: function (model) {
            return $http.post(baseUrl + 'api/FlowManagement/CreatePersonalFlow', model);
        },
        deleteDepartmentFlow: function (departmentId, flowTemplateId) {
            return $http['delete'](baseUrl + 'api/FlowManagement/DeleteDepartmentFlow', {
                params: {
                    departmentId: departmentId,
                    flowTemplateId: flowTemplateId
                }
            });
        },
        deletePersonalFlow: function (employeeId, flowTemplateId) {
            return $http['delete'](baseUrl + 'api/FlowManagement/DeletePersonalFlow', {
                params: {
                    employeeId: employeeId,
                    flowTemplateId: flowTemplateId
                }
            });
        },
        getEmployeeFlowSetting: function (flowTemplateId, departmentId, groupId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetEmployeeFlowSetting', {
                params: {
                    flowTemplateId: flowTemplateId,
                    departmentId: departmentId,
                    groupId: groupId
                }
            });
        },
        getUserPermissionOnFlowManagement: function () {
            return $http.get(baseUrl + 'api/FlowManagement/GetUserPermissionOnFlowManagement');
        },
        getDepartmentHasSetting: function (flowTemplateId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetDepartmentHasSetting', { params: { flowTemplateId: flowTemplateId } });
        }
    };
}]);

flowMngModule.service('cacheService', function () {
    var property;

    return {
        getProperty: function () {
            return property;
        },
        setProperty: function (value) {
            property = value;
        }
    };
});