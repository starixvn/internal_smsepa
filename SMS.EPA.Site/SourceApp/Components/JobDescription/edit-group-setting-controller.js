(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditGroupSettingController', editGroupSettingController);
    editGroupSettingController.$inject = ['$routeParams', 'CommonModel', 'JobDescriptionModel', 'lockScreenService', 'notificationFactory', 'historyFactory'];

    function editGroupSettingController($routeParams, CommonModel, JobDescriptionModel, lockScreenService, notificationFactory, historyFactory) {
        let vm = this;

        vm.groupId = parseInt($routeParams.id);
        vm.originalJobLevelId = 0;
        vm.jobLevelId = 0;
        vm.originalSelectedItems = [];
        vm.settingInfo = {};
        vm.jobLevels = [];

        function getSelectedItems() {
            const items = [];
            angular.forEach(vm.settingInfo.GroupItems, function (groupItem) {
                angular.forEach(groupItem.Items, function (item) {
                    if (item.IsSelected) {
                        items.push(item);
                    }
                });
            });

            return items;
        }

        function getGroupSettingById() {
            lockScreenService.start();
            JobDescriptionModel.getGroupSettingById(vm.groupId, vm.jobLevelId).then(function () {
                vm.settingInfo = JobDescriptionModel.groupSettingInfo;
                vm.originalSelectedItems = getSelectedItems();

                lockScreenService.stop();
            });
        }

        function initialize() {
            CommonModel.getJobLevel().then(
                function () {
                    vm.jobLevels = CommonModel.jobLevels;

                    if (vm.jobLevels.length > 0) {
                        vm.jobLevelId = vm.jobLevels[0].JobLevelId;
                        vm.originalJobLevelId = vm.jobLevelId;

                        getGroupSettingById();
                    }
                }
            );
        }

        function validate() {
            let isValid = true;
            const selectedItems = getSelectedItems();
            if (selectedItems.length === 0) {
                notificationFactory.error('', 'Please select at least one description item!');
                isValid = false;
            }

            return isValid;
        }

        vm.backToList = function () {
            historyFactory.back();
        };

        vm.handleChangeJobLevel = function () {
            const selectedItems = getSelectedItems();
            if (vm.originalSelectedItems.length !== selectedItems.length
                || vm.originalSelectedItems.filter(x => selectedItems.findIndex(y => y.ItemId === x.ItemId) < 0).length > 0) {
                JobDescriptionModel.openPopupConfirmRefreshData(vm).then(function () {
                    vm.originalJobLevelId = vm.jobLevelId;
                    getGroupSettingById();
                }, function () {
                    vm.jobLevelId = vm.originalJobLevelId;
                })['finally'](function () {
                    vm.modalInstance = '';
                });
            } else {
                vm.originalJobLevelId = vm.jobLevelId;
                getGroupSettingById();
            }
        };

        vm.createDescriptionGroupItem = function () {
            JobDescriptionModel.openPopupCreateDescriptionGroupItem(vm, vm.groupId, null).then(function (isSuccessful) {
                if (isSuccessful) {
                    getGroupSettingById();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.editDescriptionGroupItem = function (groupItem) {
            groupItem.GroupId = vm.groupId;
            JobDescriptionModel.openPopupEditDescriptionGroupItem(vm, groupItem).then(function (editedGroupItem) {
                groupItem.Name = editedGroupItem.Name;
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.deleteDescriptionGroupItem = function (groupItem) {
            JobDescriptionModel.openPopupDeleteDescriptionGroupItem(vm, groupItem);
        };

        vm.createDescriptionItem = function (groupItemId) {
            JobDescriptionModel.openPopupCreateDescriptionItem(vm, groupItemId).then(function (isSuccessful) {
                if (isSuccessful) {
                    getGroupSettingById();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.editDescriptionItem = function (item) {
            JobDescriptionModel.openPopupEditDescriptionItem(vm, item).then(function (editedItem) {
                item.Name = editedItem.Name;
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.deleteDescriptionItem = function (item) {
            JobDescriptionModel.openPopupDeleteDescriptionItem(vm, item);
        };

        vm.handleSelectAllGroupItem = function (isSelectedAll, groupItem) {
            angular.forEach(groupItem.Items, function (item) {
                item.IsSelected = isSelectedAll;
            });
        }

        vm.handleSelectItem = function (groupItem) {
            const selectedItems = groupItem.Items.filter(x => x.IsSelected);
            if (groupItem.Items.length == selectedItems.length) {
                groupItem.IsSelectedAll = true;
            } else {
                groupItem.IsSelectedAll = false;
            }
        };

        vm.save = function () {
            const isValid = validate();
            if (isValid) {
                const setting = {
                    groupId: vm.groupId,
                    jobLevelId: vm.jobLevelId,
                    items: []
                };

                angular.forEach(vm.settingInfo.GroupItems, function (groupItem) {
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

                JobDescriptionModel.editGroupSetting(setting).success(function (response) {
                    if (response.IsSuccessful) {
                        vm.originalSelectedItems = getSelectedItems();
                    }
                });
            }
        };

        initialize();
    }
})(angular);