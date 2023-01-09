(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('jobLevelStandardPeriodFactory', periodFactory);
    periodFactory.$inject = ['$http', 'baseUrl'];

    function periodFactory($http, baseUrl) {
        return {
            getPermission,
            getPeriods,
            getPeriodById,
            createPeriod,
            editPeriod,
            editStatusPeriod,
            deletePeriod,
            getEmployeeSettings,
            addEmployeeSetting,
            updateJobLevel,
            editEmployeeSetting,
            editStatusEmployeeSetting,
            editAssessmentStatus,
            getJobLevelSettings,
            getJobLevelSettingBySectionId,
            editJobLevelSetting,
            copyDefaultJobLevelSetting
        };

        function getPermission() {
            return $http.get(baseUrl + 'api/JobLevelStandardPeriod/GetPermission');
        }

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

        function getEmployeeSettings(params) {
            return $http.post(baseUrl + 'api/JobLevelStandardPeriod/GetEmployeeSettings', params);
        }

        function addEmployeeSetting(params) {
            return $http.put(baseUrl + 'api/JobLevelStandardPeriod/AddEmployeeSetting', params);
        }

        function updateJobLevel(params) {
            return $http.put(baseUrl + 'api/JobLevelStandardPeriod/UpdateJobLevel', params);
        }

        function editEmployeeSetting(params) {
            return $http.put(baseUrl + 'api/JobLevelStandardPeriod/EditEmployeeSetting', params);
        }

        function editStatusEmployeeSetting(params) {
            return $http.put(baseUrl + 'api/JobLevelStandardPeriod/EditStatusEmployeeSetting', params);
        }

        function editAssessmentStatus(params) {
            return $http.put(baseUrl + 'api/JobLevelAssessment/EditStatus', params);
        }

        function getJobLevelSettings(params) {
            return $http.post(baseUrl + 'api/JobLevelStandardPeriod/GetJobLevelSettings', params);
        }

        function getJobLevelSettingBySectionId(params) {
            return $http.post(baseUrl + 'api/JobLevelStandardPeriod/GetJobLevelSettingBySectionId', params);
        }

        function editJobLevelSetting(params) {
            return $http.put(baseUrl + 'api/JobLevelStandardPeriod/EditJobLevelSetting', params);
        }

        function copyDefaultJobLevelSetting(params) {
            return $http.post(baseUrl + 'api/JobLevelStandardPeriod/CopyDefaultJobLevelSetting', params);
        }
    }
})(angular);