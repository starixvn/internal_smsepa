(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').factory('JobDescriptionModel', jobDescriptionModel);
    jobDescriptionModel.$inject = ['$uibModal', 'jobDescriptionFactory', 'errorFactory'];

    function jobDescriptionModel($uibModal, jobDescriptionFactory, errorFactory) {
        var model = {
            permission: 0,
            groupSettingInfos: [],
            groupSettingInfo: {},
            employeeSettingInfos: [],
            employeeSettingInfo: {},
            getUserPermission: getUserPermission,
            getGroupSettingInfo: getGroupSettingInfo,
            getGroupSettingById: getGroupSettingById,
            getEmployeeSettingInfo: getEmployeeSettingInfo,
            getEmployeeSettingById: getEmployeeSettingById,
            openPopupCreateDescriptionGroupItem: openPopupCreateDescriptionGroupItem
        };

        return model;

        function getUserPermission() {
            return jobDescriptionFactory.getUserPermission().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getGroupSettingInfo() {
            return jobDescriptionFactory.getGroupSettingInfo()
                .success(function (groupSettingInfos) {
                    model.groupSettingInfos = groupSettingInfos;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getGroupSettingById(groupId, jobLevelId) {
            return jobDescriptionFactory.getGroupSettingById(groupId, jobLevelId)
                .success(function (groupSettingInfo) {
                    model.groupSettingInfo = groupSettingInfo;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getEmployeeSettingInfo(groupId, employeeId, showAll) {
            return jobDescriptionFactory.getEmployeeSettingInfo(groupId, employeeId, showAll)
                .success(function (employeeSettingInfos) {
                    model.employeeSettingInfos = employeeSettingInfos;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getEmployeeSettingById(employeeId) {
            return jobDescriptionFactory.getEmployeeSettingById(employeeId)
                .success(function (employeeSettingInfo) {
                    model.employeeSettingInfo = employeeSettingInfo;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function openPopupCreateDescriptionGroupItem(vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/create-job-description-group-item.html',
                controller: 'CreateDescriptionGroupItemController as createGroupItemCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }
    }
})(angular);