(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JobLevelStandardPeriodController', periodController);
    periodController.$inject = ['$location', 'JobLevelStandardPeriodModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService', 'baseUrl'];

    function periodController($location, PeriodModel, CommonModel, utilitiesFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.periods = [];
        vm.filter = { isActive: 'true' };
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.permission = PeriodModel.permission;
        vm.policy = utilitiesFactory.policy;
        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);

        function getPeriods(isActive) {
            lockScreenService.start();

            PeriodModel.getPeriods(isActive).then(function () {
                vm.periods = PeriodModel.periods;
                utilitiesFactory.searchOnPage(vm, vm.periods);
                lockScreenService.stop();
            });
        }

        vm.query = function (filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getPeriods(filter.isActive);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.create = function () {
            PeriodModel.openPopupCreatePeriod(vm).then(function () {
                vm.modalInstance = null;
                getPeriods(vm.filter.isActive);
            });
        };

        vm.edit = function (periodId) {
            $location.path(baseUrl + 'JobLevelStandard/Period/Edit/' + periodId);
        };

        vm.editActive = function (period) {
            if (vm.permission === vm.policy.Host) {
                PeriodModel.openPopupEditStatusPeriod(period);
            }
        };

        vm.delete = function (period) {
            PeriodModel.openPopupDeletePeriod(vm, period);
        };

        function initialize() {
            PeriodModel.getPermission().then(function () {
                vm.permission = PeriodModel.permission;

                getPeriods(vm.filter.isActive);
            });
        }

        initialize();
    }
})(angular);