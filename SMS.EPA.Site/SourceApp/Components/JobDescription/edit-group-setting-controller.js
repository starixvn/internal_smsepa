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
            JobDescriptionModel.openPopupEditDescriptionGroupItem(vm, groupItem).then(function (isSuccessful) {
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

        vm.createDescriptionItem = function (groupItemId) {
            JobDescriptionModel.openPopupCreateDescriptionItem(vm, groupItemId).then(function (isSuccessful) {
                if (isSuccessful) {
                    getGroupSettingById();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        vm.editDescriptionItem = function (item) {
            JobDescriptionModel.openPopupEditDescriptionItem(vm, item).then(function (isSuccessful) {
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

        vm.handleSelectAllGroupItem = function (isSelectedAll, groupItem) {
            angular.forEach(groupItem.Items, function (item) {
                item.IsSelected = isSelectedAll;
            });
        }

        vm.save = function () {
            const setting = {
                groupId: vm.groupId,
                jobLevelId: vm.jobLevelId,
                items: []
            };

            angular.forEach(vm.groupSettingInfo.GroupItems, function (groupItem) {
                angular.forEach(groupItem.Items, function (item) {
                    if (item.IsSelected) {
                        setting.items.push({
                            groupItemId: groupItem.GroupItemId,
                            itemId: item.ItemId,
                            displayOrder: setting.items.length + 1
                        });
                    }
                });
            });

            JobDescriptionModel.editGroupSetting(setting);
        }

        initialize();
    }
})(angular);