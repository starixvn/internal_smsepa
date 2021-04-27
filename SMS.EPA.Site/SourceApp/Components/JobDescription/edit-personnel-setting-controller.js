(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditPersonnelSettingController', editPersonnelSettingController);
    editPersonnelSettingController.$inject = ['$routeParams', 'JobDescriptionModel', 'lockScreenService'];

    function editPersonnelSettingController($routeParams, JobDescriptionModel, lockScreenService) {
        var vm = this;

        vm.employeeId = parseInt($routeParams.id);
        vm.employeeSettingInfo = {};

        vm.createDescriptionGroupItem = function () {
            JobDescriptionModel.openPopupCreateDescriptionGroupItem(vm).then()['finally'](function () {
                vm.modalInstance = '';
            });
        }

        initialize();

        function getEmployeeSettingById() {
            lockScreenService.start();
            JobDescriptionModel.getEmployeeSettingById(vm.employeeId).then(
                function () {
                    vm.employeeSettingInfo = JobDescriptionModel.employeeSettingInfo;
                    lockScreenService.stop();
                });
        }

        function initialize() {
            getEmployeeSettingById();
        }
    }
})();