'use strict';
/*global periodModule*/

periodModule.controller('PeriodController',
['PeriodModel', 'CommonModel', 'utilitiesFactory', 'enumerationsFactory', 'historyFactory', 'baseUrl',
function (PeriodModel, CommonModel, utilitiesFactory, enumerationsFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.periods = PeriodModel.periods;
    vm.targetTypes = CommonModel.targetTypes;
    vm.permission = PeriodModel.permission;

    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;

    vm.filter = function initFilter() {
        var filter = historyFactory.getState();

        return filter ? filter : {
            targetTypeId: null,
            isActive: 'true'
        };
    };

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    CommonModel.getTargetType(true, vm.targetTypeEnum.Monthly).then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    var getPeriod = function (targetTypeId, isActive) {
        PeriodModel.getPeriod(targetTypeId, isActive).then(function () {
            vm.periods = PeriodModel.periods;
            utilitiesFactory.searchOnPage(vm, vm.periods);
        });
        historyFactory.saveState(vm.filter);
    };

    getPeriod(vm.filter.targetTypeId, vm.filter.isActive);

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
        vm.updatePeriodUrl = baseUrl + 'Period/Update/' + periodId;
    };

    vm.confirmActive = function (period) {
        PeriodModel.openPopupUpdateStatusPeriod(period);
    };

    vm.delete = function (period) {
        PeriodModel.openPopupDeletePeriod(vm, period);
    };

    PeriodModel.getPermissionOnPeriod().then(function () {
        vm.permission = PeriodModel.permission;
    });

    vm.viewEmployeeSetting = function (period) {
        vm.viewEmployeeSettingUrl = baseUrl + 'Period/Update/' + period.PeriodId + '?viewSetting';
    };
}]);

periodModule.controller('CreatePeriodController',
['$modalInstance', 'PeriodModel', 'CommonModel', 'enumerationsFactory',
function ($modalInstance, PeriodModel, CommonModel, enumerationsFactory) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.targetTypeEnum = enumerationsFactory.targetType;

    CommonModel.getTargetType(true, vm.targetTypeEnum.Monthly).then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    vm.period = {
        Name: null,
        TargetTypeId: null,
        StartDate: null,
        EndDate: null,
        RawStartDate: null,
        RawEndDate: null,
        AttendanceStartDate: null,
        AttendanceEndDate: null,
        IsActive: true,
        Description: null
    };

    vm.calendar = PeriodModel.calendar;

    vm.openPopupStartDate = function (event) {
        PeriodModel.openPopupStartDate(event);
    };

    vm.openPopupEndDate = function (event) {
        PeriodModel.openPopupEndDate(event);
    };

    vm.openPopupAttendanceStartDate = function (event) {
        PeriodModel.openPopupAttendanceStartDate(event);
    };

    vm.openPopupAttendanceEndDate = function (event) {
        PeriodModel.openPopupAttendanceEndDate(event);
    };

    vm.save = function (period) {
        if (PeriodModel.validatePeriod(period)) {
            period.StartDate = period.RawStartDate ? period.RawStartDate.toDateString() : null;
            period.EndDate = period.RawEndDate ? period.RawEndDate.toDateString() : null;
            period.AttendanceStartDate = period.RawAttendanceStartDate ? period.RawAttendanceStartDate.toDateString() : null;
            period.AttendanceEndDate = period.RawAttendanceEndDate ? period.RawAttendanceEndDate.toDateString() : null;

            PeriodModel.createPeriod(period, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);

periodModule.controller('UpdatePeriodController',
['$window', '$routeParams', 'PeriodModel', 'enumerationsFactory', 'historyFactory',
function ($window, $routeParams, PeriodModel, enumerationsFactory, historyFactory) {
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

    PeriodModel.getPeriodById(vm.periodId).then(function () {
        vm.period = PeriodModel.period;
        vm.period.TargetTypeId = vm.period.TargetType.TargetTypeId;
    });

    vm.backToList = function () {
        historyFactory.back();
    };

    PeriodModel.getPermissionOnPeriod().then(function () {
        vm.permission = PeriodModel.permission;
    });
}]);

periodModule.controller('UpdateBasicInformationPeriodController',
['$scope', '$window', 'PeriodModel', 'CommonModel', 'enumerationsFactory', 'historyFactory',
function ($scope, $window, PeriodModel, CommonModel, enumerationsFactory, historyFactory) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.period = {};
    vm.isFirstLoad = true;

    vm.targetTypeEnum = enumerationsFactory.targetType;

    vm.calendar = PeriodModel.calendar;

    vm.openPopupStartDate = function (event) {
        PeriodModel.openPopupStartDate(event);
    };

    vm.openPopupEndDate = function (event) {
        PeriodModel.openPopupEndDate(event);
    };

    vm.openPopupAttendanceStartDate = function (event) {
        PeriodModel.openPopupAttendanceStartDate(event);
    };

    vm.openPopupAttendanceEndDate = function (event) {
        PeriodModel.openPopupAttendanceEndDate(event);
    };

    var getTargetType = function () {
        CommonModel.getTargetType(true, vm.targetTypeEnum.Monthly).then(function () {
            vm.targetTypes = CommonModel.targetTypes;
            vm.isFirstLoad = false;
        });
    };

    $scope.$watch(function () { return $scope.updateCtrl.period; },
        function (newVal) {
            if (newVal) {
                vm.period = $scope.updateCtrl.period;

                vm.period.RawStartDate = vm.period.StartDate ? new Date(vm.period.StartDate) : null;
                vm.period.RawEndDate = vm.period.EndDate ? new Date(vm.period.EndDate) : null;
                vm.period.RawAttendanceStartDate = vm.period.AttendanceStartDate ? new Date(vm.period.AttendanceStartDate) : null;
                vm.period.RawAttendanceEndDate = vm.period.AttendanceEndDate ? new Date(vm.period.AttendanceEndDate) : null;

                if ($scope.updateCtrl.dataTab.basicInformation && vm.isFirstLoad && vm.period.PeriodId) {
                    getTargetType();
                }
            }
        });

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.basicInformation; },
        function (newVal) {
            if (newVal && vm.isFirstLoad && vm.period.PeriodId) {
                getTargetType();
            }
        });

    vm.save = function (period) {
        if (PeriodModel.validatePeriod(period)) {
            period.StartDate = period.RawStartDate ? period.RawStartDate.toDateString() : null;
            period.EndDate = period.RawEndDate ? period.RawEndDate.toDateString() : null;
            period.AttendanceStartDate = period.RawAttendanceStartDate ? period.RawAttendanceStartDate.toDateString() : null;
            period.AttendanceEndDate = period.RawAttendanceEndDate ? period.RawAttendanceEndDate.toDateString() : null;

            PeriodModel.updateBasicInformationPeriod(period);
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };
}]);

periodModule.controller('ProcessSettingPeriodController',
['$scope', '$window', 'PeriodModel', 'CommonModel', 'historyFactory',
function ($scope, $window, PeriodModel, CommonModel, historyFactory) {
    var vm = this;

    vm.period = {};
    vm.isFirstLoad = true;
    vm.processSettings = PeriodModel.processSettings;
    vm.remindDay = CommonModel.remindDay;

    var getProcessSetting = function (periodId) {
        PeriodModel.getProcessSettingByPeriodId(periodId).then(function () {
            vm.processSettings = PeriodModel.processSettings;
            vm.isFirstLoad = false;
        });
    };

    $scope.$watch(function () { return $scope.updateCtrl.period; },
        function (newVal) {
            if (newVal) {
                vm.period = $scope.updateCtrl.period;

                if ($scope.updateCtrl.dataTab.processSetting && vm.isFirstLoad && vm.period.PeriodId) {
                    getProcessSetting(vm.period.PeriodId);
                }
            }
        });

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.processSetting; },
    function (newVal) {
        if (newVal && vm.isFirstLoad && vm.period) {
            getProcessSetting(vm.period.PeriodId);
        }
    });

    vm.save = function () {
        var periodProcessSetting = {
            PeriodId: vm.period.PeriodId,
            Processes: vm.processSettings
        };

        PeriodModel.updatePeriodProcessSetting(periodProcessSetting);
    };

    vm.cancel = function () {
        historyFactory.back();
    };
}]);