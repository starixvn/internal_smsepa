'use strict';
/*global epaHistoryModule*/

epaHistoryModule.controller('EPAHistoryController',
['$window', '$routeParams', 'EPAHistoryModel', 'utilitiesFactory', 'CommonModel', 'baseUrl',
function ($window, $routeParams, EPAHistoryModel, utilitiesFactory, CommonModel, baseUrl) {
    var vm = this;
    vm.epas = EPAHistoryModel.epas;
    vm.periodOptions = CommonModel.periodOptions;
    vm.onlyEmployeeRole = EPAHistoryModel.onlyEmployeeRole;

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

    CommonModel.getAllPeriodIncludeImport().then(function () {
        vm.periodOptions = CommonModel.periodOptions;
    });

    CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
        vm.filter.employee = CommonModel.employee;
        vm.filter.employee.Name = vm.filter.employee.NickName;
        vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
    });

    EPAHistoryModel.checkHasOnlyEmployeeRole().then(function () {
        vm.onlyEmployeeRole = EPAHistoryModel.onlyEmployeeRole;
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
        EPAHistoryModel.getEPAHistory(periodId, employeeId).then(function () {
            vm.epas = EPAHistoryModel.epas;
            utilitiesFactory.searchOnPage(vm, vm.epas);
        });
    };

    getEPAHistory(vm.filter.periodId, vm.filter.employeeId);

    vm.query = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        getEPAHistory(filter.periodId, employeeId);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.backToList = function () {
        $window.history.back();
    };

    vm.viewDetailEPA = function (epaId) {
        vm.viewEPADetailUrl = baseUrl + 'EPA/Detail/' + epaId;
    };
}]);