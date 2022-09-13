(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditPeriodJobLevelSettingController', editJobLevelSettingController);
    editJobLevelSettingController.$inject = ['$uibModalInstance', 'JobLevelStandardPeriodModel',
        'utilitiesFactory', 'lockScreenService', 'permission', 'periodId', 'section'];

    function editJobLevelSettingController($uibModalInstance, PeriodModel, utilitiesFactory, lockScreenService, permission, periodId, section) {
        let vm = this;

        vm.permission = permission;
        vm.policy = utilitiesFactory.policy;
        vm.section = section;
        vm.settings = [];

        function getSettings() {
            lockScreenService.start();

            PeriodModel.getJobLevelSettingBySectionId({ periodId, sectionId: section.SectionId }).then(function () {
                vm.settings = PeriodModel.sectionJobLevelSettings;
                lockScreenService.stop();
            });
        }

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.save = function () {
            const command = {
                periodId,
                sectionIds: vm.section.SectionId,
                settings: vm.settings
            };
            PeriodModel.editJobLevelSetting(command, $uibModalInstance);
        };

        getSettings();
    }
})(angular);