(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').factory('jobDescriptionFactory', jobDescriptionFactory);
    jobDescriptionFactory.$inject = ['$http', 'baseUrl'];

    function jobDescriptionFactory($http, baseUrl) {
        return {
            getUserPermission: getUserPermission,
            getGroupSettingInfo: getGroupSettingInfo,
            getGroupSettingById: getGroupSettingById,
            getEmployeeSettingInfo: getEmployeeSettingInfo,
            getEmployeeSettingById: getEmployeeSettingById,
            createJobDescriptionGroupItem: createJobDescriptionGroupItem
        };

        function getUserPermission() {
            return $http.get(baseUrl + 'api/JobDescription/GetUserPermission');
        }

        function getGroupSettingInfo() {
            return $http.post(baseUrl + 'api/JobDescription/GetGroupSettingInfo', {});
        }

        function getGroupSettingById(groupId, jobLevelId) {
            return $http.get(baseUrl + 'api/JobDescription/GetGroupSettingById', { params: { groupId: groupId, jobLevelId: jobLevelId } });
        }

        function getEmployeeSettingInfo(groupId, employeeId, showAll) {
            return $http.post(baseUrl + 'api/JobDescription/GetEmployeeSettingInfo', {
                groupId: groupId,
                employeeId: employeeId,
                showAll: showAll
            });
        }

        function getEmployeeSettingById(employeeId) {
            return $http.get(baseUrl + 'api/JobDescription/GetEmployeeSettingById', { params: { employeeId: employeeId } });
        }

        function createJobDescriptionGroupItem(criteria) {
            return $http.post(baseUrl + 'api/JobDescription/CreateJobDescriptionGroupItem', criteria);
        }
    }
})(angular);