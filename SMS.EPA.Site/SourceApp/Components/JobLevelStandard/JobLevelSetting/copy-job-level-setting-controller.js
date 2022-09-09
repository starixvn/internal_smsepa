(function (angular) {
    'use strict';

    angular.module('jobLevelStandardModule').controller('CopyJobLevelSettingController', copyJobLevelSettingController);
    copyJobLevelSettingController.$inject = ['$uibModalInstance', 'notificationFactory', 'JobLevelSettingModel', 'selectedSection', 'sections'];

    function copyJobLevelSettingController($uibModalInstance, notificationFactory, JobLevelSettingModel, selectedSection, sections) {
        let vm = this;

        vm.selectedSection = selectedSection;
        vm.sections = [];
        vm.selectSectionIds = '';

        function initialize() {
            angular.forEach(sections, function (section) {
                const index = vm.sections.findIndex(x => x.Id === section.DepartmentId);
                const item = {
                    Id: section.SectionId,
                    Text: section.Name,
                    Enabled: true,
                    Checked: false,
                    IsLeaf: true,
                    ParentId: section.DepartmentId,
                    Items: []
                };
                if (index === -1) {
                    vm.sections.push({
                        Id: section.DepartmentId,
                        Text: section.Department,
                        Enabled: true,
                        Checked: false,
                        IsLeaf: false,
                        ParentId: null,
                        Items: [item]
                    });
                } else {
                    vm.sections[index].Items.push(item);
                }
            });
        }

        function validate() {
            let isValid = true;
            if (vm.selectSectionIds === '') {
                isValid = false;
                notificationFactory.error('', 'Please select section you want to copy setting.');
            }

            return isValid;
        }

        vm.onSelectSection = function (sectionIds) {
            vm.selectSectionIds = sectionIds;
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.save = function () {
            const isValid = validate();
            if (isValid) {
                JobLevelSettingModel.getBySectionId(vm.selectedSection.SectionId).then(function () {
                    const settings = JobLevelSettingModel.settings;

                    const command = {
                        sectionIds: vm.selectSectionIds,
                        settings: settings
                    };
                    JobLevelSettingModel.editJobLevelSetting(command, $uibModalInstance);
                });
            }
        };

        initialize();
    }
})(angular);