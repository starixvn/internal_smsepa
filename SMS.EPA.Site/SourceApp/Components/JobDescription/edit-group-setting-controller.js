(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditGroupSettingController', editGroupSettingController);
    editGroupSettingController.$inject = ['$routeParams', 'CommonModel', 'JobDescriptionModel', 'lockScreenService', 'historyFactory'];

    function editGroupSettingController($routeParams, CommonModel, JobDescriptionModel, lockScreenService, historyFactory) {
        var vm = this;

        vm.groupId = parseInt($routeParams.id);
        vm.jobLevelId = 0;
        vm.groupSettingInfo = {};
        vm.jobLevels = [];

        vm.backToList = function () {
            historyFactory.back();
        };

        vm.handleChangeJobLevel = function () {
            getGroupSettingById();
        }

        vm.createDescriptionGroupItem = function () {
            JobDescriptionModel.openPopupCreateDescriptionGroupItem(vm, vm.groupId, null).then(function (isSuccessful) {
                if (isSuccessful) {
                    getGroupSettingById();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        vm.editDescriptionGroupItem = function (groupItem) {
            JobDescriptionModel.openPopupUpdateDescriptionGroupItem(vm, groupItem).then(function (isSuccessful) {
                if (isSuccessful) {
                    getGroupSettingById();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        vm.createDescriptionItem = function (groupItemId) {
            JobDescriptionModel.openPopupCreateDescriptionItem(vm, groupItemId).then(function (isSuccessful) {
                if (isSuccessful) {
                    getGroupSettingById();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        vm.deleteDescriptionGroupItem = function (groupItem) {
            JobDescriptionModel.openPopupDeleteDescriptionGroupItem(vm, groupItem);
        }

        vm.editDescriptionItem = function (item) {
            JobDescriptionModel.openPopupUpdateDescriptionItem(vm, item).then(function (isSuccessful) {
                if (isSuccessful) {
                    getGroupSettingById();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        vm.deleteDescriptionItem = function (item) {
            JobDescriptionModel.openPopupDeleteDescriptionItem(vm, item);
        }

        function getGroupSettingById() {
            lockScreenService.start();
            JobDescriptionModel.getGroupSettingById(vm.groupId, vm.jobLevelId).then(function () {
                vm.groupSettingInfo = JobDescriptionModel.groupSettingInfo;
                lockScreenService.stop();
            });
        }

        function initialize() {
            CommonModel.getJobLevel().then(
                function () {
                    vm.jobLevels = CommonModel.jobLevels;

                    if (vm.jobLevels.length > 0) {
                        vm.jobLevelId = vm.jobLevels[0].JobLevelId;

                        getGroupSettingById();
                    }
                }
            );
        }

        initialize();
    }
})(angular);