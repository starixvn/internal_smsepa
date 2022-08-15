(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditJobLevelSettingController', editJobLevelSettingController);
    editJobLevelSettingController.$inject = ['$uibModalInstance', 'JobLevelSettingModel', 'lockScreenService', 'section'];

    function editJobLevelSettingController($uibModalInstance, JobLevelSettingModel, lockScreenService, section) {
        let vm = this;

        vm.section = section;
        vm.settings = [];

        function getSettings() {
            lockScreenService.start();

            JobLevelSettingModel.getBySectionId(vm.section.SectionId).then(function () {
                vm.settings = JobLevelSettingModel.settings;
                lockScreenService.stop();
            });
        }

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.save = function () {
            const command = {
                SectionId: vm.section.SectionId,
                Settings: vm.settings
            };
            JobLevelSettingModel.editJobLevelSetting(command, $uibModalInstance);
        };

        getSettings();
    }
})(angular);