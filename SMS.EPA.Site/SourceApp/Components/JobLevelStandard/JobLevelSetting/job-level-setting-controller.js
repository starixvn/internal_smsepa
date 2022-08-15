(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JobLevelSettingController', jobLevelSettingController);
    jobLevelSettingController.$inject = ['JobLevelSettingModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function jobLevelSettingController(JobLevelSettingModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.sections = [];
        vm.modalInstance = '';
        vm.filter = { hasSetting: '' };
        vm.currentPage = 1;
        vm.pageSize = 20;
        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);

        function getSettings(filter) {
            lockScreenService.start();

            JobLevelSettingModel.getSettings(filter).then(function () {
                vm.sections = JobLevelSettingModel.sections;
                utilitiesFactory.searchOnPage(vm, vm.sections);
                lockScreenService.stop();
            });
        }

        vm.query = function (filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getSettings(filter);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.edit = function (section) {
            JobLevelSettingModel.openPopupEditJobLevelSetting(vm, section).then(function (isSuccessful) {
                if (isSuccessful) {
                    getSettings(vm.filter);
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.copy = function (section) {
            let sectionHasSettings = vm.sections.filter(x => x.SectionId !== section.SectionId && x.HasSetting);
            JobLevelSettingModel.openPopupCopyJobLevelSetting(vm, section, sectionHasSettings).then(function (isSuccessful) {
                if (isSuccessful) {
                    getSettings(vm.filter);
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        function initialize() {
            getSettings(vm.filter);
        }

        initialize();
    }
})(angular);