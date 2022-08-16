(function (angular) {
    'use strict';

    angular.module('jobLevelStandardModule').controller('CopyPeriodJobLevelSettingController', copyJobLevelSettingController);
    copyJobLevelSettingController.$inject = ['$uibModalInstance', 'notificationFactory', 'JobLevelStandardPeriodModel', 'periodId', 'selectedSection', 'sectionHasSettings'];

    function copyJobLevelSettingController($uibModalInstance, notificationFactory, JobLevelStandardPeriodModel, periodId, selectedSection, sectionHasSettings) {
        let vm = this;

        vm.selectedSection = selectedSection;
        vm.sectionHasSettings = sectionHasSettings;
        vm.fromSection = '';

        function validate() {
            let isValid = true;
            if (vm.fromSection === '') {
                isValid = false;
                notificationFactory.error('', 'Please select section you want to copy setting.');
            }

            return isValid;
        }

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.save = function () {
            const isValid = validate();
            if (isValid) {
                JobLevelStandardPeriodModel.getJobLevelSettingBySectionId({ periodId, sectionId: vm.fromSection.SectionId }).then(function () {
                    const command = {
                        periodId,
                        sectionId: vm.selectedSection.SectionId,
                        settings: JobLevelStandardPeriodModel.sectionJobLevelSettings
                    };
                    JobLevelStandardPeriodModel.editJobLevelSetting(command, $uibModalInstance);
                });
            }
        };
    }
})(angular);