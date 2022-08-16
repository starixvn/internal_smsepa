(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('jobLevelStandardPeriodFactory', periodFactory);
    periodFactory.$inject = ['$http', 'baseUrl'];

    function periodFactory($http, baseUrl) {
        return {
            getPeriods,
            getPeriodById,
            createPeriod,
            editPeriod,
            editStatusPeriod,
            deletePeriod,
            getJobLevelSettings,
            getJobLevelSettingBySectionId,
            editJobLevelSetting,
            copyDefaultJobLevelSetting
        };

        function getPeriods(isActive) {
            return $http.post(baseUrl + 'api/JobLevelStandardPeriod/GetPeriods', { isActive });
        }

        function getPeriodById(periodId) {
            return $http.get(baseUrl + 'api/JobLevelStandardPeriod/GetById', { params: { periodId } });
        }

        function createPeriod(period) {
            return $http.post(baseUrl + 'api/JobLevelStandardPeriod/Create', period);
        }

        function editPeriod(period) {
            return $http.put(baseUrl + 'api/JobLevelStandardPeriod/Edit', period);
        }

        function editStatusPeriod(period) {
            return $http.put(baseUrl + 'api/JobLevelStandardPeriod/EditStatus', period);
        }

        function deletePeriod(periodId) {
            return $http['delete'](baseUrl + 'api/JobLevelStandardPeriod/Delete', { params: { periodId } });
        }

        function getJobLevelSettings(params) {
            return $http.post(baseUrl + 'api/JobLevelStandardPeriod/GetJobLevelSettings', params);
        }

        function getJobLevelSettingBySectionId(params) {
            return $http.post(baseUrl + 'api/JobLevelStandardPeriod/GetJobLevelSettingBySectionId', params);
        }

        function editJobLevelSetting(command) {
            return $http.put(baseUrl + 'api/JobLevelStandardPeriod/EditJobLevelSetting', command);
        }

        function copyDefaultJobLevelSetting(periodId) {
            return $http.post(baseUrl + 'api/JobLevelStandardPeriod/CopyDefaultJobLevelSetting', { periodId });
        }
    }
})(angular);