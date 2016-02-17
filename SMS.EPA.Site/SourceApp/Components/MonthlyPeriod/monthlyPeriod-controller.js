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

    vm.update = function (periodId) {
        vm.updatePeriodUrl = baseUrl + 'MonthlyPeriod/Update/' + periodId;
    };

    vm.delete = function (period) {
        MonthlyPeriodModel.openPopupDeleteMonthlyPeriod(vm, period);
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

periodModule.controller('UpdateMonthlyPeriodController',
['$window', '$routeParams', 'MonthlyPeriodModel', 'PeriodModel',
function ($window, $routeParams, MonthlyPeriodModel, PeriodModel) {
    var vm = this;

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
        $window.history.back();
    };

    MonthlyPeriodModel.getPermissionOnMonthlyPeriod().then(function () {
        vm.permission = MonthlyPeriodModel.permission;
    });
}]);

periodModule.controller('UpdateBasicInfoMonthlyPeriodController',
['$scope', '$window', 'MonthlyPeriodModel',
function ($scope, $window, MonthlyPeriodModel) {
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
            if (newVal ) {
                vm.period = $scope.updateCtrl.period;
            }
        });

    vm.save = function (period) {
        if (MonthlyPeriodModel.validateMonthlyPeriod(period)) {
            MonthlyPeriodModel.updateBasicInfoMonthlyPeriod(period);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };
}]);

periodModule.controller('UpdateProcessSettingMonthlyPeriodController',
['$scope', '$window', 'MonthlyPeriodModel', 'CommonModel',
function ($scope, $window, MonthlyPeriodModel, CommonModel) {
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
        $window.history.back();
    };
}]);

periodModule.controller('EmployeeSettingMonthlyPeriodController',
['$scope', '$routeParams', '$window', 'PeriodModel', 'utilitiesFactory', 'CommonModel', 'notificationFactory',
function ($scope, $routeParams, $window, PeriodModel, utilitiesFactory, CommonModel, notificationFactory) {
    var vm = this;

    vm.period = null;
    vm.periodId = null;
    vm.employeeSettings = PeriodModel.employeeSettings;
    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;
    vm.isFirstLoad = true;
    vm.departmentOptions = [];

    vm.selectedAll = false;

    vm.filter = {
        periodId: null,
        departmentId: null,
        isLocal: '',
        monthlyPeriod: new Date(),
        isExcludedEmployee: null,
        isGeneratedEPA: "",
        isEvaluated: ""
    };

    vm.openPopupMonthlyPeriod = function (event) {
        event.preventDefault();
        event.stopPropagation();

        vm.calendar.monthlyPeriodOpened = true;
    };

    vm.initialize = function () {
        vm.calendar = {
            monthlyPeriodOpened: false
        };

        vm.currentPage = 1;
        vm.pageSize = 20;

        vm.dateOptions = {
            minMode: 'month'
        };

        utilitiesFactory.numberEntryList(vm);
    };

    vm.initialize();
}]);