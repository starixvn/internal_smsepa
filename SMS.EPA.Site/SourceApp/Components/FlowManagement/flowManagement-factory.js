'use strict';
/*global flowMngModule*/

flowMngModule.factory('flowMngFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getFlow: function (departmentId, flowType) {
            return $http.get(baseUrl + 'api/FlowManagement/GetFlow', { params: {
                departmentId: departmentId,
                flowType: flowType
            }
            });
        },
        createFlow: function (model) {
            return $http.post('/api/FlowManagement/CreateFlow', model);
        },
        deleteFlow: function (departmentId, flowType) {
            return $http['delete']('/api/FlowManagement/DeleteFlow', { params: {
                departmentId: departmentId,
                flowType: flowType
            }
            });
        }
    };
} ]);