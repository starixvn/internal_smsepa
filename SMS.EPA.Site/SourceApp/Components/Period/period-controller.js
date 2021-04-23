(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('periodModule').controller('PeriodController', periodController);
    periodController.$inject = ['$location', 'PeriodModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl'];

    function periodController($location, PeriodModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.periods = [];
        vm.targetTypes = [];
        vm.permission = PeriodModel.permission;
        vm.policyEnum = utilitiesFactory.policy;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.filter = {};
        vm.currentPage = 1;
        vm.pageSize = 20;
        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);

        function initFilter() {
            var filter = historyFactory.getState();

            return filter ? filter : {
                targetTypeId: null,
                isActive: 'true'
            };
        }

        function getPeriod(targetTypeId, isActive) {
            lockScreenService.start();

            PeriodModel.getPeriod(targetTypeId, isActive).then(function () {
                vm.periods = PeriodModel.periods;
                utilitiesFactory.searchOnPage(vm, vm.periods);
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        }

        vm.query = function (filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getPeriod(filter.targetTypeId, filter.isActive);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.create = function () {
            PeriodModel.openPopupCreatePeriod(vm).then(function () {
                vm.modalInstance = null;
                getPeriod(vm.filter.targetTypeId, vm.filter.isActive);
            });
        };

        vm.update = function (periodId) {
            $location.path(baseUrl + 'Period/Update/' + periodId);
        };

        vm.confirmActive = function (period) {
            PeriodModel.openPopupUpdateStatusPeriod(period);
        };

        vm.delete = function (period) {
            PeriodModel.openPopupDeletePeriod(vm, period);
        };

        vm.viewEmployeeSetting = function (period, inprogress) {
            $location.path(baseUrl + 'Period/Update/' + period.PeriodId).search({
                inprogress: inprogress,
                viewSetting: true
            });
        };

        function initialize() {
            vm.filter = initFilter();

            PeriodModel.getPermissionOnPeriod().then(function () {
                vm.permission = PeriodModel.permission;
            });

            CommonModel.getTargetType(true, vm.targetTypeEnum.Monthly).then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });

            getPeriod(vm.filter.targetTypeId, vm.filter.isActive);
        }

        initialize();
    }
})(angular);