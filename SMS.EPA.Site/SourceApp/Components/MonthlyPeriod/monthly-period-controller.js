(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('monthlyPeriodModule').controller('MonthlyPeriodController', monthlyPeriodController);
    monthlyPeriodController.$inject = ['$location', 'MonthlyPeriodModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl'];

    function monthlyPeriodController($location, MonthlyPeriodModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        function initFilter() {
            var filter = historyFactory.getState();

            return filter ? filter : {
                fromDate: new Date(new Date().getFullYear(), 0, 1),
                toDate: new Date()
            };
        }

        function getMonthlyPeriod(fromDate, toDate) {
            lockScreenService.start();
            toDate = new Date(toDate.getFullYear(), toDate.getMonth() + 1, 0);

            MonthlyPeriodModel.getMonthlyPeriod(fromDate, toDate).then(function () {
                vm.periods = MonthlyPeriodModel.periods;
                utilitiesFactory.searchOnPage(vm, vm.periods);
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        }

        function initialize() {
            vm.calendar = {
                fromDateOpened: false,
                toDateOpened: false
            };
            vm.currentPage = 1;
            vm.dateOptions = {
                minMode: 'month'
            };
            vm.filter = initFilter();
            vm.pageSize = 20;
            vm.periods = MonthlyPeriodModel.periods;
            vm.permission = MonthlyPeriodModel.permission;
            vm.policyEnum = utilitiesFactory.policy;
            utilitiesFactory.numberEntryList(vm);

            MonthlyPeriodModel.getPermissionOnMonthlyPeriod().then(function () {
                vm.permission = MonthlyPeriodModel.permission;
            });

            getMonthlyPeriod(vm.filter.fromDate, vm.filter.toDate);
        }

        initialize();

        vm.openPopupFromDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.fromDateOpened = true;
            vm.calendar.toDateOpened = false;
        };

        vm.openPopupToDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.fromDateOpened = false;
            vm.calendar.toDateOpened = true;
        };

        vm.query = function (filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getMonthlyPeriod(filter.fromDate, filter.toDate);
        };

        vm.create = function () {
            MonthlyPeriodModel.openPopupCreateMonthlyPeriod(vm).then(function () {
                vm.modalInstance = null;
                getMonthlyPeriod(vm.filter.fromDate, vm.filter.toDate);
            });
        };

        vm.update = function (periodId) {
            $location.path(baseUrl + 'MonthlyPeriod/Update/' + periodId);
        };

        vm.delete = function (period) {
            MonthlyPeriodModel.openPopupDeleteMonthlyPeriod(vm, period);
        };

        vm.viewEmployeeSetting = function (period, inprogress) {
            $location.path(baseUrl + 'MonthlyPeriod/Update/' + period.PeriodId + '?viewSetting&inprogress=' + inprogress);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };
    }
})(angular);