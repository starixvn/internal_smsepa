'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.controller('MonthlyPeriodController',
['MonthlyPeriodModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function (MonthlyPeriodModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this,
        date = new Date();

    vm.filter = {
        fromDate: new Date(date.getFullYear(), 0, 1),
        toDate: date
    };

    var getMonthlyPeriod = function (fromDate, toDate) {
        toDate = new Date(toDate.getFullYear(), toDate.getMonth() + 1, 0);

        MonthlyPeriodModel.getMonthlyPeriod(fromDate, toDate).then(function () {
            vm.periods = MonthlyPeriodModel.periods;
            utilitiesFactory.searchOnPage(vm, vm.periods);
        });
    };

    vm.initialize = function () {
        vm.periods = MonthlyPeriodModel.periods;
        vm.permission = MonthlyPeriodModel.permission;
        vm.calendar = {
            fromDateOpened: false,
            toDateOpened: false
        };

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
        getMonthlyPeriod(vm.filter.fromDate, vm.filter.toDate);
    };

    vm.create = function () {
        MonthlyPeriodModel.openPopupCreateMonthlyPeriod(vm).then(function () {
            vm.modalInstance = null;
            getMonthlyPeriod(vm.filter.fromDate, vm.filter.toDate);
        });
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
}]);

monthlyPeriodModule.controller('CreateMonthlyPeriodController',
['$modalInstance', 'MonthlyPeriodModel',
function ($modalInstance, MonthlyPeriodModel) {
    var vm = this,
        date = new Date();

    vm.period = {
        Name: null,
        StartDate: new Date(date.getFullYear(), date.getMonth(), 1),
        EndDate: new Date(date.getFullYear(), date.getMonth() + 1, 0),
        Description: null
    };

    vm.calendar = {
        startDateOpened: false,
        endDateOpened: false
    };

    vm.openPopupStartDate = function (event) {
        MonthlyPeriodModel.openPopupStartDate(event, vm);
    };

    vm.openPopupEndDate = function (event) {
        MonthlyPeriodModel.openPopupEndDate(event, vm);
    };

    vm.save = function (period) {
        if (MonthlyPeriodModel.validateMonthlyPeriod(period)) {
            period.StartDate = period.StartDate.toDateString();
            period.EndDate = period.EndDate.toDateString();

            MonthlyPeriodModel.createMonthlyPeriod(period, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);