(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').factory('jobDescriptionFactory', jobDescriptionFactory);
    jobDescriptionFactory.$inject = ['$http', 'baseUrl'];

    function jobDescriptionFactory($http, baseUrl) {
        return {
            getUserPermission: getUserPermission,
            getGroupSettingInfo: getGroupSettingInfo,
            getGroupHasSetting: getGroupHasSetting,
            getGroupSettingById: getGroupSettingById,
            editGroupSetting: editGroupSetting,
            copyGroupSetting: copyGroupSetting,
            getEmployeeSettingInfo: getEmployeeSettingInfo,
            getEmployeeHasSetting: getEmployeeHasSetting,
            getEmployeeSettingById: getEmployeeSettingById,
            editEmployeeSetting: editEmployeeSetting,
            copyEmployeeSetting: copyEmployeeSetting,
            createJobDescriptionGroupItem: createJobDescriptionGroupItem,
            editJobDescriptionGroupItem: editJobDescriptionGroupItem,
            deleteJobDescriptionGroupItem: deleteJobDescriptionGroupItem,
            createJobDescriptionItem: createJobDescriptionItem,
            editJobDescriptionItem: editJobDescriptionItem,
            deleteJobDescriptionItem: deleteJobDescriptionItem
        };

        function getUserPermission() {
            return $http.get(baseUrl + 'api/JobDescription/GetUserPermission');
        }

        function getGroupSettingInfo() {
            return $http.post(baseUrl + 'api/JobDescription/GetGroupSettingInfo', {});
        }

        function getGroupHasSetting() {
            return $http.get(baseUrl + 'api/JobDescription/GetGroupHasSetting');
        }

        function getGroupSettingById(groupId, jobLevelId) {
            return $http.get(baseUrl + 'api/JobDescription/GetGroupSettingById', { params: { groupId: groupId, jobLevelId: jobLevelId } });
        }

        function editGroupSetting(setting) {
            return $http.post(baseUrl + 'api/JobDescription/EditGroupSetting', setting);
        }

        function copyGroupSetting(setting) {
            return $http.post(baseUrl + 'api/JobDescription/CopyGroupSetting', setting);
        }

        function getEmployeeSettingInfo(groupId, employeeId, showAll) {
            return $http.post(baseUrl + 'api/JobDescription/GetEmployeeSettingInfo', {
                groupId: groupId,
                employeeId: employeeId,
                showAll: showAll
            });
        }

        function getEmployeeHasSetting() {
            return $http.get(baseUrl + 'api/JobDescription/GetEmployeeHasSetting',);
        }

        function getEmployeeSettingById(employeeId) {
            return $http.get(baseUrl + 'api/JobDescription/GetEmployeeSettingById', { params: { employeeId: employeeId } });
        }

        function editEmployeeSetting(setting) {
            return $http.post(baseUrl + 'api/JobDescription/EditEmployeeSetting', setting);
        }

        function copyEmployeeSetting(setting) {
            return $http.post(baseUrl + 'api/JobDescription/CopyEmployeeSetting', setting);
        }

        function createJobDescriptionGroupItem(groupItem) {
            return $http.post(baseUrl + 'api/JobDescription/CreateJobDescriptionGroupItem', groupItem);
        }

        function createJobDescriptionItem(item) {
            return $http.post(baseUrl + 'api/JobDescription/CreateJobDescriptionItem', item);
        }

        function editJobDescriptionGroupItem(groupItem) {
            return $http.post(baseUrl + 'api/JobDescription/EditJobDescriptionGroupItem', groupItem);
        }

        function editJobDescriptionItem(item) {
            return $http.post(baseUrl + 'api/JobDescription/EditJobDescriptionItem', item);
        }

        function deleteJobDescriptionGroupItem(groupItemId) {
            return $http['delete'](baseUrl + 'api/JobDescription/DeleteJobDescriptionGroupItem', { params: { groupItemId: groupItemId } });
        }

        function deleteJobDescriptionItem(itemId) {
            return $http['delete'](baseUrl + 'api/JobDescription/DeleteJobDescriptionItem', { params: { itemId: itemId } });
        }
    }
})(angular);