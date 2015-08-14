'use strict';
/*global periodModule*/

periodModule.controller('PeriodController',
['PeriodModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function (PeriodModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this;

    vm.periods = PeriodModel.periods;
    vm.targetTypes = CommonModel.targetTypes;
    vm.periodDateFormat = PeriodModel.periodDateFormat;

    vm.filter = {
        targetTypeId: null,
        isActive: 'true'
    };

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);

    CommonModel.getTargetType().then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    var getPeriod = function (targetTypeId, isActive) {
        PeriodModel.getPeriod(targetTypeId, isActive).then(function () {
            vm.periods = PeriodModel.periods;
            utilitiesFactory.searchOnPage(vm, vm.periods);
        });
    };

    getPeriod(vm.filter.targetTypeId, vm.filter.isActive);

    vm.query = function (filter) {
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
}]);

periodModule.controller('CreatePeriodController',
['$modalInstance', 'PeriodModel', 'CommonModel',
function ($modalInstance, PeriodModel, CommonModel) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.period = {
        Name: null,
        TargetTypeId: null,
        RawStartDate: new Date(),
        StartDate: null,
        IsActive: true,
        Description: null
    };

    vm.calendar = PeriodModel.calendar;
    vm.openPopupStartDate = function (event) {
        PeriodModel.openPopupStartDate(event);
    };

    vm.save = function (period) {
        if (PeriodModel.validatePeriod(period)) {
            vm.period.StartDate = vm.period.RawStartDate.getDate() + ' ' + PeriodModel.getShortNameMonth(vm.period.RawStartDate.getMonth());
            PeriodModel.createPeriod(period, $modalInstance);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);

periodModule.controller('UpdatePeriodController',
['$window', '$routeParams', 'PeriodModel', 
function ($window, $routeParams, PeriodModel) {
    var vm = this;

    vm.period = {};
    vm.periodId = parseInt($routeParams.id);
    vm.date = new Date();

    vm.dataTab = {
        basicInformation: true,
        employeeSetting: false
    };

    vm.tabBasicSelect = function () {
        vm.dataTab.basicInformation = true;
        vm.dataTab.employeeSetting = false;
    };

    vm.employeeSetting = function () {
        vm.dataTab.basicInformation = false;
        vm.dataTab.employeeSetting = true;
    };

    PeriodModel.getPeriodById(vm.periodId).then(function () {
        vm.period = PeriodModel.period;
        vm.period.TargetTypeId = vm.period.TargetType.TargetTypeId;
        var startDate = vm.period.StartDate + ', ' + vm.date.getFullYear();
        vm.period.RawStartDate = new Date(startDate);
    });

    vm.backToList = function () {
        $window.history.back();
    };
}]);

periodModule.controller('UpdateBasicInformationPeriodController',
['$scope', '$window', 'PeriodModel', 'CommonModel',
function ($scope, $window, PeriodModel, CommonModel) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.period = {};
    vm.isFirstLoad = true;

    vm.calendar = PeriodModel.calendar;
    vm.openPopupStartDate = function (event) {
        PeriodModel.openPopupStartDate(event);
    };

    $scope.$watch(function () { return $scope.updateCtrl.period; },
        function (newVal) {
            if (newVal) {
                vm.period = $scope.updateCtrl.period;
            }
        });

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.basicInformation; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                CommonModel.getTargetType().then(function () {
                    vm.targetTypes = CommonModel.targetTypes;
                    vm.isFirstLoad = false;
                });
            }
        });

    vm.save = function (period) {
        if (PeriodModel.validatePeriod(period)) {
            vm.period.StartDate = vm.period.RawStartDate.getDate() + ' ' + PeriodModel.getShortNameMonth(vm.period.RawStartDate.getMonth());
            PeriodModel.updateBasicInformationPeriod(period);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };
}]);

periodModule.controller('UpdateEmployeeSettingPeriodController',
['$scope', '$window', 'PeriodModel', 'utilitiesFactory', 'CommonModel', 'EmployeeModel', 'FlowMngModel',
function ($scope, $window, PeriodModel, utilitiesFactory, CommonModel, EmployeeModel, FlowMngModel) {
    var vm = this;

    vm.period = {};
    vm.employeeSettings = PeriodModel.employeeSettings;
    vm.flowTemplates = FlowMngModel.flowTemplates;
    vm.departments = CommonModel.departments;
    vm.isFirstLoad = true;

    vm.currentPage = 1;
    vm.pageSize = 15;
    utilitiesFactory.numberEntryList(vm);

    vm.filter = {
        departmentId: '',
        employeeId: '',
        flowTemplateId: ''
    };

    $scope.$watch(function () { return $scope.updateCtrl.period; },
    function (newVal) {
        if (newVal) {
            vm.period = $scope.updateCtrl.period;
        }
    });

    $scope.$watch(function () { return $scope.updateCtrl.dataTab.employeeSetting; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                CommonModel.getDepartment(null, true).then(function () {
                    vm.departments = CommonModel.departments;
                });

                FlowMngModel.getFlowTemplate(vm.period.TargetType.TargetTypeId, true).then(function () {
                    vm.flowTemplates = FlowMngModel.flowTemplates;
                });

                PeriodModel.getPeriodEmployeeSettingById(vm.period.PeriodId).then(function () {
                    vm.employeeSettings = PeriodModel.employeeSettings;
                    utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
                    vm.isFirstLoad = false;
                });
            }
        });

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.getEmployee = function () {
        return EmployeeModel.getAllEmployee(vm.searchEmployee);
    };

    vm.getEmployee = function () {
        return EmployeeModel.getAllEmployee(vm.searchEmployee);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filter.employee) {
                vm.filter.employee = null;
            }
        }
    };
}]);