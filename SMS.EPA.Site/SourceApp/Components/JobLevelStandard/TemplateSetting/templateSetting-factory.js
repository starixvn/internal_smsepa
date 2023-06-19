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
            assignTemplate
        };

        function assignTemplate(params) {
            return $http.put(baseUrl + 'api/JobLevelStandardTemplateSetting/Assign', params);
        }

        function getTemplate() {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplate/GetTemplate', {
                params: {
                    isActive: true
                }
            });
        }

        function deleteTemplateSetting(objectId, objectTypeId) {
            return $http['delete'](baseUrl + 'api/JobLevelStandardTemplateSetting/Delete', {
                params: { objectId, objectTypeId }
            });
        }

        function getTemplateSetting(objectId, objectTypeId, showAll) {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplateSetting/GetTemplateSetting', {
                params: { objectId, objectTypeId, showAll }
            });
        }

        function getUserPermission() {
            return $http.get(baseUrl + 'api/JobLevelStandardTemplateSetting/GetUserPermission');
        }
    }
})(angular);