'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.controller('MonthlyPeriodController',
['MonthlyPeriodModel', 'CommonModel', 'utilitiesFactory', 'enumerationsFactory', 'historyFactory', 'baseUrl',
function (MonthlyPeriodModel, CommonModel, utilitiesFactory, enumerationsFactory, historyFactory, baseUrl) {
    var vm = this;
    vm.policyEnum = enumerationsFactory.policy;
    vm.filter = {};

    function initFilter() {
        var filter = historyFactory.getState();

        return filter ? filter : {
            fromDate: new Date(new Date().getFullYear(), 0, 1),
            toDate: new Date()
        };
    }

    function getMonthlyPeriod(fromDate, toDate) {
        toDate = new Date(toDate.getFullYear(), toDate.getMonth() + 1, 0);

        MonthlyPeriodModel.getMonthlyPeriod(fromDate, toDate).then(function () {
            vm.periods = MonthlyPeriodModel.periods;
            utilitiesFactory.searchOnPage(vm, vm.periods);
        });

        historyFactory.saveState(vm.filter);
    }

    vm.initialize = function () {
        vm.filter = initFilter();
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
        getMonthlyPeriod(filter.fromDate, filter.toDate);
    };

    vm.create = function () {
        MonthlyPeriodModel.openPopupCreateMonthlyPeriod(vm).then(function () {
            vm.modalInstance = null;
            getMonthlyPeriod(vm.filter.fromDate, vm.filter.toDate);
        });
    };

    vm.update = function (periodId) {
        vm.updatePeriodUrl = baseUrl + 'MonthlyPeriod/Update/' + periodId;
    };

    vm.delete = function (period) {
        MonthlyPeriodModel.openPopupDeleteMonthlyPeriod(vm, period);
    };

    vm.viewEmployeeSetting = function (period, inprogress) {
        vm.viewEmployeeSettingUrl = baseUrl + 'MonthlyPeriod/Update/' + period.PeriodId + '?viewSetting&inprogress=' + inprogress;
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

monthlyPeriodModule.controller('UpdateMonthlyPeriodController',
['$window', '$routeParams', 'MonthlyPeriodModel', 'enumerationsFactory', 'historyFactory',
function ($window, $routeParams, MonthlyPeriodModel, enumerationsFactory, historyFactory) {
    var vm = this;

    vm.policyEnum = enumerationsFactory.policy;

    vm.period = null;
    vm.periodId = parseInt($routeParams.id);
    vm.viewMode = $routeParams.viewSetting ? true : false;

    vm.date = new Date();
    vm.permission = null;

    vm.dataTab = {
        basicInformation: !vm.viewMode,
        employeeSetting: vm.viewMode,
        processSetting: false
    };

    vm.tabBasicSelect = function () {
        vm.dataTab.basicInformation = true;
        vm.dataTab.employeeSetting = false;
        vm.dataTab.processSetting = false;
    };

    vm.tabEmployeeSettingSelect = function () {
        vm.dataTab.basicInformation = false;
        vm.dataTab.employeeSetting = true;
        vm.dataTab.processSetting = false;
    };

    vm.tabProcessSettingSelect = function () {
        vm.dataTab.basicInformation = false;
        vm.dataTab.employeeSetting = false;
        vm.dataTab.processSetting = true;
    };

    MonthlyPeriodModel.getMonthlyPeriodById(vm.periodId).then(function () {
        vm.period = MonthlyPeriodModel.period;
    });

    vm.backToList = function () {
        historyFactory.back();
    };

    MonthlyPeriodModel.getPermissionOnMonthlyPeriod().then(function () {
        vm.permission = MonthlyPeriodModel.permission;
    });
}]);

monthlyPeriodModule.controller('UpdateBasicInfoMonthlyPeriodController',
['$scope', '$window', 'MonthlyPeriodModel', 'historyFactory',
function ($scope, $window, MonthlyPeriodModel, historyFactory) {
    var vm = this;

    vm.period = {};

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

    $scope.$watch(function () { return $scope.updateCtrl.period; },
        function (newVal) {
            if (newVal) {
                vm.period = $scope.updateCtrl.period;
            }
        });

    vm.save = function (period) {
        if (MonthlyPeriodModel.validateMonthlyPeriod(period)) {
            MonthlyPeriodModel.updateBasicInfoMonthlyPeriod(period);
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };
}]);

monthlyPeriodModule.controller('UpdateProcessSettingMonthlyPeriodController',
['$scope', '$window', 'MonthlyPeriodModel', 'CommonModel', 'historyFactory',
function ($scope, $window, MonthlyPeriodModel, CommonModel, historyFactory) {
    var vm = this;

    vm.period = {};
    vm.processSettings = MonthlyPeriodModel.processSettings;
    vm.remindDay = CommonModel.remindDay;
    vm.isFirstLoad = true;

    var getProcessSetting = function (periodId) {
        MonthlyPeriodModel.getProcessSettingByPeriodId(periodId).then(function () {
            vm.processSettings = MonthlyPeriodModel.processSettings;
            vm.isFirstLoad = false;
        });
    };

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.processSetting; },
    function (newVal) {
        if (newVal && vm.isFirstLoad) {
            vm.period = $scope.updateCtrl.period;
            getProcessSetting(vm.period.PeriodId);
        }
    });

    vm.save = function () {
        var processSetting = {
            PeriodId: vm.period.PeriodId,
            Processes: vm.processSettings
        };

        MonthlyPeriodModel.updateProcessSettingMonthlyPeriod(processSetting);
    };

    vm.cancel = function () {
        historyFactory.back();
    };
}]);