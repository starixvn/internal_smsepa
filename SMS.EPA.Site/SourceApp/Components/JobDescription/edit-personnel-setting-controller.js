(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditPersonnelSettingController', editPersonnelSettingController);
    editPersonnelSettingController.$inject = ['$routeParams', 'JobDescriptionModel', 'lockScreenService', 'CommonModel', 'historyFactory'];

    function editPersonnelSettingController($routeParams, JobDescriptionModel, lockScreenService, CommonModel, historyFactory) {
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
        vm.updateStatusDescriptionGroupItem = updateStatusDescriptionGroupItem;
        vm.updateStatusDescriptionItem = updateStatusDescriptionItem;

        initialize();

        vm.backToList = function () {
            historyFactory.back();
        };

         function getIndex (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getEmployeeSettingById() {
            lockScreenService.start();
            JobDescriptionModel.getEmployeeSettingById(vm.employeeId).then(
                function () {
                    vm.employeeSettingInfo = JobDescriptionModel.employeeSettingInfo;
                    lockScreenService.stop();
                });
        }

        function updateStatusDescriptionGroupItem(groupItem) {
            JobDescriptionModel.openPopupUpdateStatusDescriptionGroupItem(groupItem);
        }

        function updateStatusDescriptionItem(item) {
            JobDescriptionModel.openPopupUpdateStatusDescriptionItem(item);
        }

        function createDescriptionGroupItem() {
            JobDescriptionModel.openPopupCreateDescriptionGroupItem(vm, null, vm.employeeId).then()['finally'](function () {
                vm.modalInstance = '';
                getEmployeeSettingById();
            });
        }

        function updateDescriptionGroupItem(groupItem) {
            JobDescriptionModel.openPopupUpdateDescriptionGroupItem(vm, groupItem).then(function (updatedDescriptionGroupItem) {
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
            JobDescriptionModel.openPopupUpdateDescriptionItem(vm, item).then(function (updatedDescriptionItem) {
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