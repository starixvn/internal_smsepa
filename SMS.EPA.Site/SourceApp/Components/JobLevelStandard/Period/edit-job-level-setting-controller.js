(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditPeriodJobLevelSettingController', editJobLevelSettingController);
    editJobLevelSettingController.$inject = ['$uibModalInstance', 'JobLevelStandardPeriodModel', 'lockScreenService', 'periodId', 'section'];

    function editJobLevelSettingController($uibModalInstance, JobLevelStandardPeriodModel, lockScreenService, periodId, section) {
        let vm = this;

        vm.section = section;
        vm.settings = [];

        function getSettings() {
            lockScreenService.start();

            JobLevelStandardPeriodModel.getJobLevelSettingBySectionId({ periodId, sectionId: section.SectionId }).then(function () {
                vm.settings = JobLevelStandardPeriodModel.sectionJobLevelSettings;
                lockScreenService.stop();
            });
        }

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.save = function () {
            const command = {
                periodId,
                sectionId: vm.section.SectionId,
                settings: vm.settings
            };
            JobLevelStandardPeriodModel.editJobLevelSetting(command, $uibModalInstance);
        };

        getSettings();
    }
})(angular);