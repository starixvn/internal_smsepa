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
            createJobDescriptionGroupItem: createJobDescriptionGroupItem,
            updateJobDescriptionGroupItem: updateJobDescriptionGroupItem,
            deleteJobDescriptionGroupItem: deleteJobDescriptionGroupItem,
            createJobDescriptionItem: createJobDescriptionItem,
            updateJobDescriptionItem: updateJobDescriptionItem,
            deleteJobDescriptionItem: deleteJobDescriptionItem,
            updateEmployeeSettingGroupItem: updateEmployeeSettingGroupItem,
            updateEmployeeSettingItem: updateEmployeeSettingItem,
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

        function createJobDescriptionGroupItem(groupItem) {
            return $http.post(baseUrl + 'api/JobDescription/CreateJobDescriptionGroupItem', groupItem);
        }

        function createJobDescriptionItem(item) {
            return $http.post(baseUrl + 'api/JobDescription/CreateJobDescriptionItem', item);
        }

        function updateJobDescriptionGroupItem(groupItem) {
            return $http.post(baseUrl + 'api/JobDescription/UpdateJobDescriptionGroupItem', groupItem);
        }

        function updateJobDescriptionItem(item) {
            return $http.post(baseUrl + 'api/JobDescription/UpdateJobDescriptionItem', item);
        }

        function deleteJobDescriptionGroupItem(groupItemId) {
            return $http['delete'](baseUrl + 'api/JobDescription/DeleteJobDescriptionGroupItem', { params: { groupItemId: groupItemId } });
        }

        function deleteJobDescriptionItem(itemId) {
            return $http['delete'](baseUrl + 'api/JobDescription/DeleteJobDescriptionItem', { params: { itemId: itemId } });
        }

        function updateEmployeeSettingGroupItem(groupItem) {
            return $http.put(baseUrl + 'api/JobDescription/UpdateEmployeeSettingGroupItem', groupItem);
        }

        function updateEmployeeSettingItem(item) {
            return $http.put(baseUrl + 'api/JobDescription/UpdateEmployeeSettingItem', item);
        }
    }
})(angular);