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
        getDepartmentFlow: function (departmentId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetDepartmentFlow', { params: { departmentId: departmentId } });
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