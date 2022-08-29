(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('jobLevelSettingFactory', jobLevelSettingFactory);
    jobLevelSettingFactory.$inject = ['$http', 'baseUrl'];

    function jobLevelSettingFactory($http, baseUrl) {
        return {
            getPermission,
            getSettings,
            getBySectionId,
            edit
        };

        function getPermission() {
            return $http.get(baseUrl + 'api/JobLevelSetting/GetPermission');
        }

        function getSettings(params) {
            return $http.post(baseUrl + 'api/JobLevelSetting/GetSettings', params);
        }

        function getBySectionId(sectionId) {
            return $http.get(baseUrl + 'api/JobLevelSetting/GetBySectionId', { params: { sectionId } });
        }

        function edit(command) {
            return $http.put(baseUrl + 'api/JobLevelSetting/Edit', command);
        }
    }
})(angular);