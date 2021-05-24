(function (angular) {
    'use strict';

    angular.module('jobLevelModule').controller('JobLevelController', JobLevelController);
    JobLevelController.$inject = ['JobLevelModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function JobLevelController(JobLevelModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.permission = 0;
        vm.policy = utilitiesFactory.policy;

        vm.createJobLevel = createJobLevel;
        vm.editJobLevel = editJobLevel;
        vm.deleteJobLevel = deleteJobLevel;

        initialize();

        function createJobLevel() {
            JobLevelModel.openPopupCreateJobLevel(vm).then(function (isSuccessful) {
                if (isSuccessful) {
                    getJobLevel();
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function deleteJobLevel(jobLevel) {
            JobLevelModel.openPopupDeleteJobLevel(vm, jobLevel);
        }

        function editJobLevel(jobLevel) {
            JobLevelModel.openPopupEditJobLevel(vm, jobLevel).then(function (editJobLevel) {
                jobLevel.Name = editJobLevel.Name;
            })['finally'](function () {
                vm.modalInstance = '';
            });
        }

        function initialize() {
            JobLevelModel.getUserPermission().then(function () {
                vm.permission = JobLevelModel.permission;
            });

            getJobLevel();
        }

        function getJobLevel() {
            lockScreenService.start();
            CommonModel.getJobLevel(false).then(
                function () {
                    vm.jobLevels = CommonModel.jobLevels;
                    lockScreenService.stop();
                });
        }
    }
})(angular);