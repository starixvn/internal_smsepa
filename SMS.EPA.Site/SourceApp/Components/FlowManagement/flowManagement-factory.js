'use strict';
/*global flowMngModule*/

flowMngModule.factory('flowMngFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getFlow: function (departmentId, flowType) {
            return $http.get(baseUrl + 'api/FlowManagement/GetFlow', {
                params: {
                    departmentId: departmentId,
                    flowType: flowType
                }
            });
        },
        getFlowEmployee: function (employeeId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetFlowEmployee', { params: { employeeId: employeeId } });
        },
        createFlow: function (model) {
            return $http.post(baseUrl + 'api/FlowManagement/CreateFlow', model);
        },
        createEmployeeFlow: function(model) {
            return $http.post(baseUrl + 'api/FlowManagement/CreateEmployeeFlow', model);
        },
        deleteFlow: function (departmentId, flowType) {
            return $http['delete'](baseUrl + 'api/FlowManagement/DeleteFlow', {
                params: {
                    departmentId: departmentId,
                    flowType: flowType
                }
            });
        },
        getEmployeeSettingFlowInDepartment: function (departmentId) {
            return $http.get(baseUrl + 'api/FlowManagement/GetEmployeeSettingFlowInDepartment', { params: { departmentId: departmentId } });
        }
    };
} ]);