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
        },
        getKPIGroupOfTargetTemplate: function (targetTemplateId) {
            return $http.get('/api/TargetTemplate/GetKPIGroupOfTargetTemplate', { params: { targetTemplateId: targetTemplateId} });
        },
        addKPIGroupWeight: function (kpiGroupWeightModel) {
            return $http.post('/api/TargetTemplate/AddKPIGroupWeight', kpiGroupWeightModel);
        },
        getKPIOfTargetTemplate: function (targetTemplateId) {
            return $http.get('/api/TargetTemplate/GetKPIOfTargetTemplate', { params: { targetTemplateId: targetTemplateId} });
        },
        deleteKPIGroupOfTargetTemplate: function (targetTemplateId, kpiGroupId) {
            return $http['delete']('/api/TargetTemplate/DeleteKPIGroupOfTargetTemplate',
                        { params: { targetTemplateId: targetTemplateId, kpiGroupId: kpiGroupId } });
        }
    };
} ]);