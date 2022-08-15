(function (angular) {
    'use strict';

    angular.module('jobLevelStandardModule').controller('CopyJobLevelSettingController', copyJobLevelSettingController);
    copyJobLevelSettingController.$inject = ['$uibModalInstance', 'notificationFactory', 'JobLevelSettingModel', 'selectedSection', 'sectionHasSettings'];

    function copyJobLevelSettingController($uibModalInstance, notificationFactory, JobLevelSettingModel, selectedSection, sectionHasSettings) {
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
                JobLevelSettingModel.getBySectionId(vm.fromSection.SectionId).then(function () {
                    const settings = JobLevelSettingModel.settings;

                    const command = {
                        SectionId: vm.selectedSection.SectionId,
                        Settings: settings
                    };
                    JobLevelSettingModel.editJobLevelSetting(command, $uibModalInstance);
                });
            }
        };
    }
})(angular);