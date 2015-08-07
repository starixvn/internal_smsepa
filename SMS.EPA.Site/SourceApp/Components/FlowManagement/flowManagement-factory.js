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
        getDepartmentFlow: function (departmentId, flowTemplateId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetDepartmentFlow', {
                params: {
                    departmentId: departmentId,
                    flowTemplateId: flowTemplateId
                }
            });
        },
        getPersonalFlow: function (employeeId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetPersonalFlow', { params: { employeeId: employeeId } });
        },
        createDepartmentFlow: function (model) {
            return $http.post(baseUrl + 'api/FlowManagement/CreateDepartmentFlow', model);
        },
        createPersonalFlow: function (model) {
            return $http.post(baseUrl + 'api/FlowManagement/CreatePersonalFlow', model);
        },
        deleteDepartmentFlow: function (departmentId) {
            return $http['delete'](baseUrl + 'api/FlowManagement/DeleteDepartmentFlow', { params: { departmentId: departmentId } });
        },
        deletePersonalFlow: function (employeeId) {
            return $http['delete'](baseUrl + 'api/FlowManagement/DeletePersonalFlow', { params: { employeeId: employeeId } });
        },
        getEmployeeSettingFlowInDepartment: function (departmentId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetEmployeeSettingFlowInDepartment', { params: { departmentId: departmentId } });
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