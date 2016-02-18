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

    vm.viewEmployeeSetting = function (period) {
        vm.viewEmployeeSettingUrl = baseUrl + 'MonthlyPeriod/Update/' + period.PeriodId + '?viewSetting';
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

monthlyPeriodModule.controller('UpdateBasicInfoMonthlyPeriodController',
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

monthlyPeriodModule.controller('UpdateProcessSettingMonthlyPeriodController',
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

monthlyPeriodModule.controller('EmployeeSettingMonthlyPeriodController',
['$scope', '$routeParams', '$window', 'MonthlyPeriodModel', 'utilitiesFactory', 'CommonModel', 'notificationFactory',
function ($scope, $routeParams, $window, MonthlyPeriodModel, utilitiesFactory, CommonModel, notificationFactory) {
    var vm = this;

    vm.period = null;
    vm.periodId = null;
    vm.employeeSettings = MonthlyPeriodModel.employeeSettings;
    vm.isFirstLoad = true;
    vm.departmentOptions = [];
    vm.selectedAll = false;

    vm.currentPage = 1;
    vm.pageSize = 20;

    vm.calendar = {
        monthlyPeriodOpened: false
    };

    vm.dateOptions = {
        minMode: 'month'
    };

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    vm.filter = {
        periodId: null,
        departmentId: null,
        isLocal: '',
        monthlyPeriod: new Date(),
        isExcludedEmployee: null,
        isGeneratedEPA: "",
        isEvaluated: ""
    };

    vm.filterAssign = {
        employee: ''
    };

    vm.openPopupMonthlyPeriod = function (event) {
        event.preventDefault();
        event.stopPropagation();

        vm.calendar.monthlyPeriodOpened = true;
    };

    var getEmployeeSetting = function (filter) {
        MonthlyPeriodModel.getEmployeeSettingMonthlyPeriodById(
            filter.periodId,
            filter.departmentId,
            filter.isLocal,
            filter.isExcludedEmployee,
            filter.isGeneratedEPA,
            filter.isEvaluated,
            filter.monthlyPeriod)
        .then(function () {
            vm.employeeSettings = MonthlyPeriodModel.employeeSettings;
            utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
            vm.isFirstLoad = false;
            vm.selectedAll = false;
        });
    };

    $scope.$watch(function () {
        return $scope.updateCtrl.dataTab.employeeSetting && $scope.updateCtrl.period && $scope.updateCtrl.permission;
    },
    function (newVal) {
        if (newVal && vm.isFirstLoad) {
            vm.period = $scope.updateCtrl.period;
            vm.filter.periodId = vm.period.PeriodId;
            vm.permission = $scope.updateCtrl.permission;

            CommonModel.getDepartment(null, true).then(function () {
                var treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
                vm.departmentOptions = angular.copy(treeDepartment);
            });

            getEmployeeSetting(vm.filter);
        }
    });

    vm.query = function (filter) {
        getEmployeeSetting(filter);
    };

    vm.getEmployee = function () {
        return CommonModel.getAllEmployee(vm.searchEmployee);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filterAssign.employee) {
                vm.filterAssign.employee = null;
            }
        }
    };

    var getSelectedDepartment = function (departments) {
        var selectedDepartments = [];

        angular.forEach(departments, function (department) {
            if (department.ticked) {
                var departmentId = department.DepartmentId,
                    selectedDepartment = {
                        departmentId: departmentId < 0 ? -departmentId : departmentId,
                        isGroup: department.IsGroup
                    };

                selectedDepartments.push(selectedDepartment);
            }
        });

        return selectedDepartments;
    };

    var resetFilterAssign = function () {
        vm.filterAssign.employee = '';
        vm.searchEmployee = '';

        angular.forEach(vm.departments, function (department) {
            department.ticked = false;
        });
    };

    vm.addEmployeeToMonthlyPeriod = function (filterAssign) {
        var employeeId = null;

        if (filterAssign.employee) {
            employeeId = filterAssign.employee.EmployeeId;
        }

        var employeeInfo = {
            periodId: vm.period.PeriodId,
            employeeId: employeeId,
            departments: getSelectedDepartment(vm.departmentOptions)
        };

        if (employeeInfo.employeeId || (employeeInfo.departments && employeeInfo.departments.length > 0)) {
            MonthlyPeriodModel.addEmployeeToMonthlyPeriod(employeeInfo).then(function () {
                resetFilterAssign();
                getEmployeeSetting(vm.filter);
            });
        }
        else {
            notificationFactory.error("SelectDepartmentSectionEmployeeBeforeAddToPeriodWarning");
        }
    };

    vm.updateEmployeeSetting = function (employeeSetting) {
        MonthlyPeriodModel.openPopupUpdateEmployeeSetting(vm, new Array(employeeSetting));
    };

    vm.manageSetting = function () {
        var selectedEmployees = MonthlyPeriodModel.getSelectedEmployee(vm.employeeSettings);

        if (!selectedEmployees || selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeEditSettingWarning");
        }
        else {
            MonthlyPeriodModel.openPopupUpdateEmployeeSetting(vm, selectedEmployees);
        }
    };

    vm.generateEvaluation = function () {
        var selectedEmployees = MonthlyPeriodModel.getSelectedEmployee(vm.employeeSettings),
            selectedEmployeeInfo = MonthlyPeriodModel.getSelectedEmployeeInfo(selectedEmployees),
            period = {
                periodId: vm.period.PeriodId,
                employeeIds: selectedEmployeeInfo.selectedEmployeeIds
            };

        if (!selectedEmployees || selectedEmployees.length === 0) {
            notificationFactory.error("SelectEmployeeBeforeGenerateEvaluation");
        }
        else {
            if (MonthlyPeriodModel.validateEmployeeSettingGenerateEPA(selectedEmployees)) {
                MonthlyPeriodModel.openPopupGenerateEvaluation(vm, period).then(function () {
                    vm.query(vm.filter);
                });
            }
        }
    };

    /*
    ** Check all and sync selected employee setting between full list and filter list.
    */
    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.filteredItems, function (employeeSetting) {
            employeeSetting.Selected = selectedAll;
        });

        angular.forEach(vm.employeeSettings, function (employeeSetting) {
            employeeSetting.Selected = selectedAll;
        });
    };

    vm.syncSelectedEmployee = function (employeeId, selected) {
        var allUnchecked = true,
            allChecked = true;

        angular.forEach(vm.employeeSettings, function (employeeSetting) {
            if (employeeSetting.Employee.EmployeeId === employeeId) {
                angular.forEach(vm.filteredItems, function (filterItem) {
                    if (employeeSetting.Employee.EmployeeId === filterItem.Employee.EmployeeId) {
                        filterItem.Selected = selected;
                        employeeSetting.Selected = selected;
                    }
                });
            }
        });

        angular.forEach(vm.filteredItems, function (filterItem) {
            if (filterItem.Selected) {
                allUnchecked = false;
            }
            else {
                allChecked = false;
            }
        });

        if (allUnchecked) {
            vm.selectedAll = false;
        }

        if (allChecked) {
            vm.selectedAll = true;
        }
        else {
            vm.selectedAll = false;
        }
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
}]);