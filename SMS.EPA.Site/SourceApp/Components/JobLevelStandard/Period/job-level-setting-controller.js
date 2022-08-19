(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('PeriodJobLevelSettingController', jobLevelSettingController);
    jobLevelSettingController.$inject = ['$scope', 'JobLevelStandardPeriodModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function jobLevelSettingController($scope, PeriodModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.isFirstLoad = true;
        vm.jobLevelSettings = [];
        vm.modalInstance = '';
        vm.filter = { periodId: 0, hasSetting: '' };
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.permission = PeriodModel.permission;
        vm.policy = utilitiesFactory.policy;
        utilitiesFactory.numberEntryList(vm);

        function getJobLevelSettings(filter) {
            lockScreenService.start();

            PeriodModel.getJobLevelSettings(filter).then(function () {
                vm.jobLevelSettings = PeriodModel.jobLevelSettings;

                vm.isFirstLoad = false;
                utilitiesFactory.searchOnPage(vm, vm.jobLevelSettings);
                lockScreenService.stop();
            });
        }

        vm.query = function (filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getJobLevelSettings(filter);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.edit = function (section) {
            PeriodModel.openPopupEditJobLevelSetting(vm, section).then(function (isSuccessful) {
                if (isSuccessful) {
                    getJobLevelSettings(vm.filter);
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.copy = function (section) {
            let sectionHasSettings = vm.jobLevelSettings.filter(x => x.SectionId !== section.SectionId && x.HasSetting);
            PeriodModel.openPopupCopyJobLevelSetting(vm, section, sectionHasSettings).then(function (isSuccessful) {
                if (isSuccessful) {
                    getJobLevelSettings(vm.filter);
                }
            })['finally'](function () {
                vm.modalInstance = '';
            });
        };

        vm.copyFromDefault = function () {
            PeriodModel.openPopupCopyDefaultJobLevelSetting(vm, vm.filter.periodId);
        };

        $scope.$watch(function () { return $scope.editCtrl.period && $scope.editCtrl.dataTab === 2; },
            function (newVal) {
                if (newVal) {
                    vm.permission = $scope.editCtrl.permission;
                    vm.period = $scope.editCtrl.period;
                    vm.filter.periodId = vm.period.PeriodId;

                    if (vm.isFirstLoad && vm.period) {
                        getJobLevelSettings(vm.filter);
                    }
                }
            });
    }
})(angular);