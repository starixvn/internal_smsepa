(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditPersonnelSettingController', editPersonnelSettingController);
    editPersonnelSettingController.$inject = ['$routeParams', 'JobDescriptionModel', 'lockScreenService', 'CommonModel', 'historyFactory', 'utilitiesFactory'];

    function editPersonnelSettingController($routeParams, JobDescriptionModel, lockScreenService, CommonModel, historyFactory, utilitiesFactory) {
        var vm = this;

        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.employeeId = parseInt($routeParams.id);
        vm.employeeSettingInfo = {};
        vm.getIndex = getIndex;
        vm.createDescriptionGroupItem = createDescriptionGroupItem;
        vm.updateDescriptionGroupItem = updateDescriptionGroupItem;
        vm.deleteDescriptionGroupItem = deleteDescriptionGroupItem;
        vm.createDescriptionItem = createDescriptionItem;
        vm.updateDescriptionItem = updateDescriptionItem;
        vm.deleteDescriptionItem = deleteDescriptionItem;
        vm.updateEmployeeSettingGroupItem = updateEmployeeSettingGroupItem;
        vm.updateEmployeeSettingItem = updateEmployeeSettingItem;

        initialize();

        vm.backToList = function () {
            historyFactory.back();
        };

        utilitiesFactory.numberEntryList(vm);

         function getIndex (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getEmployeeSettingById() {
            lockScreenService.start();
            JobDescriptionModel.getEmployeeSettingById(vm.employeeId).then(
                function () {
                    vm.employeeSettingInfo = JobDescriptionModel.employeeSettingInfo;
                    utilitiesFactory.searchOnPage(vm, vm.employeeSettingInfo.GroupItems);
                    lockScreenService.stop();
                });
        }

        function updateEmployeeSettingGroupItem(groupItem, employeeId) {
            JobDescriptionModel.openPopupUpdateEmployeeSettingGroupItem(groupItem, employeeId);
        }

        function updateEmployeeSettingItem(item, employeeId) {
            JobDescriptionModel.openPopupUpdateEmployeeSettingItem(item, employeeId);
        }

        function createDescriptionGroupItem() {
            JobDescriptionModel.openPopupCreateDescriptionGroupItem(vm, null, vm.employeeId).then()['finally'](function () {
                vm.modalInstance = '';
                getEmployeeSettingById();
            });
        }

        function updateDescriptionGroupItem(groupItem) {
            JobDescriptionModel.openPopupEditDescriptionGroupItem(vm, groupItem).then(function (updatedDescriptionGroupItem) {
                groupItem.Name = updatedDescriptionGroupItem.Name;
            })['finally'](function () {
                vm.modalInstance = '';
                getEmployeeSettingById();
            });
        }

        function deleteDescriptionGroupItem(groupItem) {
            JobDescriptionModel.openPopupDeleteDescriptionGroupItem(vm, groupItem);
        }

        function createDescriptionItem(groupItemId) {
            JobDescriptionModel.openPopupCreateDescriptionItem(vm, groupItemId).then()['finally'](function () {
                vm.modalInstance = '';
                getEmployeeSettingById();
            });
        }

        function updateDescriptionItem(item) {
            JobDescriptionModel.openPopupEditDescriptionItem(vm, item).then(function (updatedDescriptionItem) {
                item.Name = updatedDescriptionItem.Name;
            })['finally'](function () {
                vm.modalInstance = '';
                getEmployeeSettingById();
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