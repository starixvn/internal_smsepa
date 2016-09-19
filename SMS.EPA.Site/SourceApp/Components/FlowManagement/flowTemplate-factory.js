(function (angular) {
    'use strict';

    angular.module('flowMngModule').factory('flowTemplateFactory', flowTemplateFactory);
    flowTemplateFactory.$inject = ['$http', 'baseUrl'];

    function flowTemplateFactory($http, baseUrl) {
        return {
            getFlowTemplate: function (targetTypeId, isActive) {
                return $http.get(baseUrl + 'api/FlowTemplate/GetFlowTemplate', {
                    params: {
                        targetTypeId: targetTypeId,
                        isActive: isActive
                    }
                });
            },
            getFlowTemplateById: function (flowTemplateId) {
                return $http.get(baseUrl + 'api/FlowTemplate/GetFlowTemplateById', { params: { flowTemplateId: flowTemplateId } });
            },
            createFlowTemplate: function (flowTemplate) {
                return $http.post(baseUrl + 'api/FlowTemplate/CreateFlowTemplate', flowTemplate);
            },
            updateFlowTemplate: function (flowTemplate) {
                return $http.put(baseUrl + 'api/FlowTemplate/UpdateFlowTemplate', flowTemplate);
            },
            updateStatusFlowTemplate: function (flowTemplate) {
                return $http.put(baseUrl + 'api/FlowTemplate/UpdateStatusFlowTemplate', flowTemplate);
            },
            deleteFlowTemplate: function (flowTemplateId) {
                return $http['delete'](baseUrl + 'api/FlowTemplate/DeleteFlowTemplate', { params: { flowTemplateId: flowTemplateId } });
            },
            getUserPermissionOnFlowManagement: function () {
                return $http.get(baseUrl + 'api/FlowTemplate/GetUserPermissionOnFlowManagement');
            }
        };
    }
})(angular);