'use strict';
/*global targetTemplateModule*/

targetTemplateModule.factory('targetTemplateFactory', ['$http', function ($http) {
    return {
        getTargetTemplate: function (targetTypeId, isActive) {
            return $http.get('/api/TargetTemplate/GetAllTargetTemplate', { params: { targetTypeId: targetTypeId, isActive: isActive} });
        },
        getTargetTemplateById: function (targetTemplateId) {
            return $http.get('/api/TargetTemplate/GetTargetTemplateById', { params: { targetTemplateId: targetTemplateId} });
        },
        createTargetTemplate: function (targetTemplate) {
            return $http.post('/api/TargetTemplate/CreateTargetTemplate', targetTemplate);
        },
        updateStatusTargetTemplate: function (targetTemplate) {
            return $http.put('/api/TargetTemplate/UpdateStatusTargetTemplate', targetTemplate);
        },
        updateTargetTemplate: function (targetTemplate) {
            return $http.put('/api/TargetTemplate/UpdateTargetTemplate', targetTemplate);
        },
        deleteTargetTemplate: function (targetTemplateId) {
            return $http['delete']('/api/TargetTemplate/DeleteTargetTemplate', { params: { targetTemplateId: targetTemplateId} });
        }
    };
} ]);