'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.controller('MonthlyPeriodController',
['MonthlyPeriodModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function (MonthlyPeriodModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this;

    vm.filter = {
        fromDate: new Date((new Date()).getFullYear(), 0, 1),
        toDate: new Date()
    };

    var getMonthlyPeriod = function (fromDate, toDate) {
        MonthlyPeriodModel.getMonthlyPeriod(fromDate, toDate).then(function () {
            vm.periods = MonthlyPeriodModel.periods;
            utilitiesFactory.searchOnPage(vm, vm.periods);
        });
    };

    vm.initialize = function () {
        vm.periods = MonthlyPeriodModel.periods;
        vm.permission = MonthlyPeriodModel.permission;
        vm.calendar = MonthlyPeriodModel.calendar;

        vm.currentPage = 1;
        vm.pageSize = 20;

        vm.dateOptions = {
            minMode: 'month'
        };

        utilitiesFactory.numberEntryList(vm);

        MonthlyPeriodModel.getPermissionOnMonthlyPeriod().then(function () {
            vm.permission = MonthlyPeriodModel.permission;
        });

        getMonthlyPeriod(vm.filter.fromDate, vm.filter.toDate);
    };

    vm.initialize();

    vm.openPopupFromDate = function (event) {
        MonthlyPeriodModel.openPopupFromDate(event);
    };

    vm.openPopupToDate = function (event) {
        MonthlyPeriodModel.openPopupToDate(event);
    };

    vm.query = function (filter) {
        vm.searchKey = '';
        vm.currentPage = 1;
        getMonthlyPeriod(vm.filter.fromDate, vm.filter.toDate);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
}]);