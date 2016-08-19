'use strict';

(function (angular) {
    angular.module('targetTemplateModule').factory('targetTemplateFactory', targetTemplateFactory);
    targetTemplateFactory.$inject = ['$http', 'baseUrl'];

    function targetTemplateFactory($http, baseUrl) {
        return {
            addKPIGroupWeight: addKPIGroupWeight,
            createTargetTemplate: createTargetTemplate,
            deleteKPIGroupOfTargetTemplate: deleteKPIGroupOfTargetTemplate,
            deleteTargetTemplate: deleteTargetTemplate,
            getKPIGroupOfTargetTemplate: getKPIGroupOfTargetTemplate,
            getKPIOfTargetTemplate: getKPIOfTargetTemplate,
            getPermissonOnTargetTemplate: getPermissonOnTargetTemplate,
            getRatingType: getRatingType,
            getScoreRatingType: getScoreRatingType,
            getTargetTemplate: getTargetTemplate,
            getTargetTemplateById: getTargetTemplateById,
            updateTargetTemplate: updateTargetTemplate,
            updateTargetTemplateBasicInfo: updateTargetTemplateBasicInfo,
            updateStatusTargetTemplate: updateStatusTargetTemplate
        };

        function addKPIGroupWeight(kpiGroupWeightModel) {
            return $http.post(baseUrl + 'api/TargetTemplate/AddKPIGroupWeight', kpiGroupWeightModel);
        }

        function createTargetTemplate(targetTemplate) {
            return $http.post(baseUrl + 'api/TargetTemplate/CreateTargetTemplate', targetTemplate);
        }

        function deleteTargetTemplate(targetTemplateId) {
            return $http['delete'](baseUrl + 'api/TargetTemplate/DeleteTargetTemplate', { params: { targetTemplateId: targetTemplateId } });
        }

        function deleteKPIGroupOfTargetTemplate(targetTemplateId, kpiGroupId) {
            return $http['delete'](baseUrl + 'api/TargetTemplate/DeleteKPIGroupOfTargetTemplate',
                        { params: { targetTemplateId: targetTemplateId, kpiGroupId: kpiGroupId } });
        }

        function getKPIGroupOfTargetTemplate(targetTemplateId) {
            return $http.get(baseUrl + 'api/TargetTemplate/GetKPIGroupOfTargetTemplate', { params: { targetTemplateId: targetTemplateId } });
        }

        function getKPIOfTargetTemplate(targetTemplateId, onlySelected) {
            return $http.get(baseUrl + 'api/TargetTemplate/GetKPIOfTargetTemplate',
                { params: { targetTemplateId: targetTemplateId, onlySelected: onlySelected } });
        }

        function getPermissonOnTargetTemplate() {
            return $http.get(baseUrl + 'api/TargetTemplate/GetUserPermissionOnTargetTemplate');
        }

        function getRatingType() {
            return $http.get(baseUrl + 'api/TargetTemplate/GetRatingType');
        }

        function getScoreRatingType(scoreRatingTypeId, isActive) {
            return $http.get(baseUrl + 'api/TargetTemplate/GetScoreRatingType',
                {
                    params: {
                        scoreRatingTypeId: scoreRatingTypeId,
                        isActive: isActive
                    }
                });
        }

        function getTargetTemplate(targetTypeId, isActive) {
            return $http.get(baseUrl + 'api/TargetTemplate/GetAllTargetTemplate', {
                params: {
                    targetTypeId: targetTypeId,
                    isActive: isActive
                }
            });
        }

        function getTargetTemplateById(targetTemplateId) {
            return $http.get(baseUrl + 'api/TargetTemplate/GetTargetTemplateById', { params: { targetTemplateId: targetTemplateId } });
        }

        function updateTargetTemplate(targetTemplate) {
            return $http.put(baseUrl + 'api/TargetTemplate/UpdateTargetTemplate', targetTemplate);
        }

        function updateTargetTemplateBasicInfo(targetTemplate) {
            return $http.put(baseUrl + 'api/TargetTemplate/UpdateTargetTemplateBasicInfo', targetTemplate);
        }

        function updateStatusTargetTemplate(targetTemplate) {
            return $http.put(baseUrl + 'api/TargetTemplate/UpdateStatusTargetTemplate', targetTemplate);
        }
    }
})(angular);