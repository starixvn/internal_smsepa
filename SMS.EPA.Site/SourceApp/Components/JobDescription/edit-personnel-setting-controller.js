(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditPersonnelSettingController', editPersonnelSettingController);
    editPersonnelSettingController.$inject = ['$routeParams', 'JobDescriptionModel', 'lockScreenService', 'CommonModel'];

    function editPersonnelSettingController($routeParams, JobDescriptionModel, lockScreenService, CommonModel) {
        var vm = this;

        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.employeeId = parseInt($routeParams.id);
        vm.employeeSettingInfo = {};
        vm.getIndex = getIndex;
        vm.createDescriptionGroupItem = createDescriptionGroupItem;
        vm.updateDescriptionGroupItem = updateDescriptionGroupItem;
        vm.deleteDescriptionGroupItem = deleteDescriptionGroupItem;
        vm.addDescriptionItem = addDescriptionItem;

        initialize();

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

        function createDescriptionGroupItem() {
            JobDescriptionModel.openPopupCreateDescriptionGroupItem(vm).then()['finally'](function () {
                vm.modalInstance = '';
                getEmployeeSettingById();
            });
        }

        function updateDescriptionGroupItem(groupItem) {
            JobDescriptionModel.openPopupUpdateDescriptionGroupItem(vm, groupItem).then(function (updatedDescriptionGroupItem) {
                groupItem.Name = updatedDescriptionGroupItem.Name;
                groupItem.IsActive = updatedDescriptionGroupItem.IsActive;
            })['finally'](function () {
                vm.modalInstance = '';
                getEmployeeSettingById();
            });
        }

        function deleteDescriptionGroupItem(groupItem) {
            JobDescriptionModel.openPopupDeleteDescriptionGroupItem(vm, groupItem);
        }

        function addDescriptionItem(groupItem) {
            JobDescriptionModel.openPopupCreateDescriptionItem(vm, groupItem).then()['finally'](function () {
                vm.modalInstance = '';
            });
        }


        function initialize() {
            getEmployeeSettingById();
        }
    }
})();