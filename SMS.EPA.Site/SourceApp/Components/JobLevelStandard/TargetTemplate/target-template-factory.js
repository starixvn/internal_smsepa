(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('jobLevelStandardTemplateFactory', templateFactory);
    templateFactory.$inject = ['$http', 'baseUrl'];

    function templateFactory($http, baseUrl) {
        return {
            createTemplate: createTemplate,
            deleteTemplate: deleteTemplate,
            getPermissionOnTemplate: getPermissionOnTemplate,
            getTemplate: getTemplate,
            getTemplateById: getTemplateById,
            updateTemplateBasicInfo: updateTemplateBasicInfo,
            updateStatusTemplate: updateStatusTemplate,
            updateTemplateKPI: updateTemplateKPI,
            getKPIListSelected: getKPIListSelected
        };

        function updateTemplateKPI(template) {
            return $http.put(baseUrl + 'api/JobLevelStandardTemplate/UpdateTemplateKPI', template);
        }

        function getKPIListSelected(id, isSelected) {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplate/GetKPIListSelected',
                { params: { templateId: id, isSelected: isSelected } });
        }

        function createTemplate(template) {
            return $http.post(baseUrl + 'api/JobLevelStandardTemplate/CreateJobLevelStandardTemplate', template);
        }

        function deleteTemplate(templateId) {
            return $http['delete'](baseUrl + 'api/JobLevelStandardTemplate/DeleteStatusJobLevelStandardTemplate', { params: { templateId: templateId } });
        }

        function getPermissionOnTemplate() {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplate/GetUserPermissionOnKPI');
        }

        function getTemplate(isActive) {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplate/GetJobLevelStandardTemplate', {
                params: {
                    isActive: isActive
                }
            });
        }

        function getTemplateById(id) {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplate/GetJobLevelStandardTemplateById', { params: { templateId: id } });
        }

        function updateTemplateBasicInfo(template) {
            return $http.put(baseUrl + 'api/JobLevelStandardTemplate/UpdateJobLevelStandardTemplate', template);
        }

        function updateStatusTemplate(template) {
            return $http.put(baseUrl + 'api/JobLevelStandardTemplate/UpdateStatusJobLevelStandardTemplate', template);
        }
    }
})(angular);