(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditPersonnelSettingController', editPersonnelSettingController);
    editPersonnelSettingController.$inject = ['$routeParams', 'JobDescriptionModel', 'lockScreenService', 'historyFactory', 'notificationFactory'];

    function editPersonnelSettingController($routeParams, JobDescriptionModel, lockScreenService, historyFactory, notificationFactory) {
        let vm = this;

        vm.employeeId = parseInt($routeParams.id);
        vm.settingInfo = {};
        vm.createDescriptionGroupItem = createDescriptionGroupItem;
        vm.updateDescriptionGroupItem = updateDescriptionGroupItem;
        vm.deleteDescriptionGroupItem = deleteDescriptionGroupItem;
        vm.createDescriptionItem = createDescriptionItem;
        vm.updateDescriptionItem = updateDescriptionItem;
        vm.deleteDescriptionItem = deleteDescriptionItem;
        vm.save = save;

        vm.backToList = function () {
            historyFactory.back();
        };

        vm.checkAll = function (isSelectedAll, groupItem) {
            angular.forEach(groupItem.Items, function (item) {
                item.IsSelected = isSelectedAll;
            });
        }

        vm.check = function (groupItem) {
            var countChecked = 0;

            angular.forEach(groupItem.Items, function (item) {
                if (item.IsSelected) {
                    countChecked++;
                }
            });

            if (groupItem.Items.length == countChecked) {
                groupItem.IsSelectedAll = true;
            }
            else {
                groupItem.IsSelectedAll = false;
            }
        };

        initialize();

        function save() {
            const isValid = validate();
            if (isValid) {
                const setting = {
                    employeeId: vm.employeeId,
                    items: []
                };

                angular.forEach(vm.settingInfo.GroupItems, function (groupItem) {
                    angular.forEach(groupItem.Items, function (item) {
                        if (item.IsSelected) {
                            setting.items.push({
                                itemId: item.ItemId,
                                displayOrder: setting.items.length + 1
                            });
                        }
                    });
                });

                JobDescriptionModel.editEmployeeSetting(setting);
            }
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

        function getEmployeeSettingById() {
            lockScreenService.start();
            JobDescriptionModel.getEmployeeSettingById(vm.employeeId).then(
                function () {
                    vm.settingInfo = JobDescriptionModel.employeeSettingInfo;
                    lockScreenService.stop();
                });
        }

        function createDescriptionGroupItem() {
            JobDescriptionModel.openPopupCreateDescriptionGroupItem(vm, null, vm.employeeId).then(function (isSuccessful) {
                if (isSuccessful) {
                    getEmployeeSettingById();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function updateDescriptionGroupItem(groupItem) {
            groupItem.EmployeeId = vm.employeeId;
            JobDescriptionModel.openPopupEditDescriptionGroupItem(vm, groupItem).then(function (updatedDescriptionGroupItem) {
                groupItem.Name = updatedDescriptionGroupItem.Name;
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function deleteDescriptionGroupItem(groupItem) {
            JobDescriptionModel.openPopupDeleteDescriptionGroupItem(vm, groupItem);
        }

        function createDescriptionItem(groupItemId) {
            JobDescriptionModel.openPopupCreateDescriptionItem(vm, groupItemId).then(function (isSuccessful) {
                if (isSuccessful) {
                    getEmployeeSettingById();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function updateDescriptionItem(item) {
            JobDescriptionModel.openPopupEditDescriptionItem(vm, item).then(function (updatedDescriptionItem) {
                item.Name = updatedDescriptionItem.Name;
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function deleteDescriptionItem(item) {
            JobDescriptionModel.openPopupDeleteDescriptionItem(vm, item);
        }

        function initialize() {
            getEmployeeSettingById();
        }
    }
})();