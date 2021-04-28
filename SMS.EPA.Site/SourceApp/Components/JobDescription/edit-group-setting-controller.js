(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('EditGroupSettingController', editGroupSettingController);
    editGroupSettingController.$inject = ['$routeParams', 'CommonModel', 'JobDescriptionModel', 'lockScreenService', 'historyFactory'];

    function editGroupSettingController($routeParams, CommonModel, JobDescriptionModel, lockScreenService, historyFactory) {
        var vm = this;

        vm.groupId = parseInt($routeParams.id);
        vm.jobLevelId = 0;
        vm.groupSettingInfo = {};
        vm.jobLevels = [];

        vm.createDescriptionGroupItem = function () {
            JobDescriptionModel.openPopupCreateDescriptionGroupItem(vm).then()['finally'](function () {
                vm.modalInstance = '';
            });
        }

        vm.backToList = function () {
            historyFactory.back();
        };

        initialize();

        function initialize() {
            CommonModel.getJobLevel().then(
                function () {
                    vm.jobLevels = CommonModel.jobLevels;

                    if (vm.jobLevels.length > 0) {
                        vm.jobLevelId = vm.jobLevels[0].JobLevelId;

                        lockScreenService.start();
                        JobDescriptionModel.getGroupSettingById(vm.groupId, vm.jobLevels[0].JobLevelId).then(
                            function () {
                                vm.groupSettingInfo = JobDescriptionModel.groupSettingInfo;
                                lockScreenService.stop();
                            });
                    }
                }
            );
        }
    }
})();