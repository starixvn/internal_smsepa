(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('JLSTemplateSettingFactory', factory);
    factory.$inject = ['$http', 'baseUrl'];

    function factory($http, baseUrl) {
        return {
            getTemplateSetting,
            getUserPermission,
            deleteTemplateSetting,
            getTemplate,
            assignTemplateToSection,
        };

        function assignTemplateToSection(params) {
            return $http.put(baseUrl + 'api/JobLevelStandardTemplateSetting/AssignJobLevelStandardTemplateSetting', params);
        }

        function getTemplate() {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplate/GetJobLevelStandardTemplate', {
                params: {
                    isActive: true
                }
            });
        }

        function deleteTemplateSetting(objectId, objectTypeId) {
            return $http['delete'](baseUrl + 'api/JobLevelStandardTemplateSetting/DeleteJobLevelStandardTemplateSetting', {
                params: { objectId, objectTypeId }
            });
        }

        function getTemplateSetting(objectId, objectTypeId, showAll) {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplateSetting/GetJobLevelStandardTemplateSetting', {
                params: { objectId, objectTypeId, showAll }
            });
        }

        function getUserPermission() {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplateSetting/GetUserPermission');
        }
    }
})(angular);