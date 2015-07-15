'use strict';
/*global targetTemplateModule*/

targetTemplateModule.factory('targetTemplateFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getTargetTemplate: function (targetTypeId, isActive) {
            return $http.get(baseUrl + 'api/TargetTemplate/GetAllTargetTemplate', { params: { targetTypeId: targetTypeId, isActive: isActive} });
        },
        getTargetTemplateById: function (targetTemplateId) {
            return $http.get(baseUrl + 'api/TargetTemplate/GetTargetTemplateById', { params: { targetTemplateId: targetTemplateId} });
        },
        createTargetTemplate: function (targetTemplate) {
            return $http.post(baseUrl + 'api/TargetTemplate/CreateTargetTemplate', targetTemplate);
        },
        updateStatusTargetTemplate: function (targetTemplate) {
            return $http.put(baseUrl + 'api/TargetTemplate/UpdateStatusTargetTemplate', targetTemplate);
        },
        updateTargetTemplate: function (targetTemplate) {
            return $http.put(baseUrl + 'api/TargetTemplate/UpdateTargetTemplate', targetTemplate);
        },
        deleteTargetTemplate: function (targetTemplateId) {
            return $http['delete'](baseUrl + '/api/TargetTemplate/DeleteTargetTemplate', { params: { targetTemplateId: targetTemplateId} });
        },
        getKPIGroupOfTargetTemplate: function (targetTemplateId) {
            return $http.get(baseUrl + 'api/TargetTemplate/GetKPIGroupOfTargetTemplate', { params: { targetTemplateId: targetTemplateId} });
        },
        addKPIGroupWeight: function (kpiGroupWeightModel) {
            return $http.post(baseUrl + 'api/TargetTemplate/AddKPIGroupWeight', kpiGroupWeightModel);
        },
        getKPIOfTargetTemplate: function (targetTemplateId) {
            return $http.get(baseUrl + 'api/TargetTemplate/GetKPIOfTargetTemplate', { params: { targetTemplateId: targetTemplateId} });
        },
        deleteKPIGroupOfTargetTemplate: function (targetTemplateId, kpiGroupId) {
            return $http['delete'](baseUrl + 'api/TargetTemplate/DeleteKPIGroupOfTargetTemplate',
                        { params: { targetTemplateId: targetTemplateId, kpiGroupId: kpiGroupId} });
        },
        getPermissonOnTargetTemplate: function () {
            return $http.get('/api/TargetTemplate/GetUserPermissionOnTargetTemplate');
        }
    };
} ]);