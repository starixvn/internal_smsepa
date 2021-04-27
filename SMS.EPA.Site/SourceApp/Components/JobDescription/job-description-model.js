(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').factory('JobDescriptionModel', jobDescriptionModel);
    jobDescriptionModel.$inject = ['$uibModal', 'jobDescriptionFactory', 'errorFactory'];

    function jobDescriptionModel($uibModal, jobDescriptionFactory, errorFactory) {
        var model = {
            permission: 0,
            employeeSettingInfos: [],
            getUserPermission: getUserPermission,
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