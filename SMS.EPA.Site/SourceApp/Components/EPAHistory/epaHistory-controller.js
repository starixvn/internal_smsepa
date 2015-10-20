'use strict'
/*global epaHistoryModule*/

epaHistoryModule.controller('EPAHistoryController',
['EPAHistoryModel', 'utilitiesFactory', 'CommonModel', 'EmployeeModel',
function (EPAHistoryModel, utilitiesFactory, CommonModel, EmployeeModel) {
    var vm = this;
    vm.epas = EPAHistoryModel.epas;
    vm.periodOptions = CommonModel.periodOptions;

    vm.filter = {
        periodId: null,
        employee: null
    };

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    CommonModel.getAllPeriodIncludeImport().then(function () {
        vm.periodOptions = CommonModel.periodOptions;
        vm.filter.periodId = vm.periodOptions.length > 0 ? vm.periodOptions[0].PeriodId : null;
        getEPAHistory(vm.filter.periodId);
    });

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

    var getEPAHistory = function (periodId, employeeId) {
        EPAHistoryModel.getEPAHistory(periodId, employeeId).then(function () {
            vm.epas = EPAHistoryModel.epas;
            utilitiesFactory.searchOnPage(vm, vm.epas);
        });
    };

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
}]);