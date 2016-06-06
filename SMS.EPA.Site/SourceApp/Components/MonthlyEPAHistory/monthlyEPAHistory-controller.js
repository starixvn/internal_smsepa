'use strict';
/*global epaHistoryModule*/

epaHistoryModule.controller('monthlyEPAHistoryController',
['$window', '$routeParams', 'monthlyEPAHistoryModel', 'utilitiesFactory', 'CommonModel', 'baseUrl', 'historyFactory', 'usSpinnerService',
function ($window, $routeParams, monthlyEPAHistoryModel, utilitiesFactory, CommonModel, baseUrl, historyFactory, usSpinnerService) {
    var vm = this;
    vm.epas = monthlyEPAHistoryModel.epas;
    vm.periodOptions = CommonModel.periodOptions;
    vm.onlyEmployeeRole = monthlyEPAHistoryModel.onlyEmployeeRole;

    vm.filter = {
        periodId: $routeParams.periodId ? $routeParams.periodId : null,
        employee: null,
        employeeId: $routeParams.empId ? $routeParams.empId : null
    };

    if (vm.filter.employeeId) {
        vm.fromEPAListing = true;
    }

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    CommonModel.getAllMonthlyPeriod().then(function () {
        vm.periodOptions = CommonModel.monthlyPeriods;
    });

    if (vm.filter.employeeId) {
        CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
            vm.filter.employee = CommonModel.employee;
            vm.filter.employee.Name = vm.filter.employee.NickName;
            vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
        });
    }

    monthlyEPAHistoryModel.checkHasOnlyEmployeeRole().then(function () {
        vm.onlyEmployeeRole = monthlyEPAHistoryModel.onlyEmployeeRole;
    });

    vm.getEmployee = function () {
        return CommonModel.getAllEmployee(vm.searchEmployee);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filter.employee) {
                vm.filter.employee = null;
            }
        }
    };

    var getEPAHistory = function (periodId, employeeId) {
        usSpinnerService.spin("spinner-1");

        monthlyEPAHistoryModel.getEPAHistory(periodId, employeeId).then(function () {
            vm.epas = monthlyEPAHistoryModel.epas;
            utilitiesFactory.searchOnPage(vm, vm.epas);
            usSpinnerService.stop('spinner-1');
        });
    };

    getEPAHistory(vm.filter.periodId, vm.filter.employeeId);

    vm.query = function () {
        var employeeId = null;

        if (vm.filter.employee) {
            employeeId = vm.filter.employee.EmployeeId;
        }

        getEPAHistory(vm.filter.periodId, employeeId);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.backToList = function () {
        historyFactory.back();
    };

    vm.viewDetailEPA = function (epaId) {
        vm.viewEPADetailUrl = baseUrl + 'EPA/Detail/' + epaId;
    };
}]);