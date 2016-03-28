'use strict';
/*global epaHistoryModule*/

epaHistoryModule.controller('EPAHistoryController',
['$window', '$routeParams', 'EPAHistoryModel', 'utilitiesFactory', 'CommonModel', 'baseUrl', 'epaHistoryViewStateService', 'usSpinnerService',
function ($window, $routeParams, EPAHistoryModel, utilitiesFactory, CommonModel, baseUrl, epaHistoryViewStateService, usSpinnerService) {
    var vm = this;
    vm.epas = EPAHistoryModel.epas;
    vm.periodOptions = CommonModel.periodOptions;
    vm.onlyEmployeeRole = EPAHistoryModel.onlyEmployeeRole;

    vm.filter = epaHistoryViewStateService;

    if (vm.filter.model.employeeId) {
        vm.fromEPAListing = true;
    }

    vm.currentPage = 1;
    vm.pageSize = 20;

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    CommonModel.getAllPeriodIncludeImport().then(function () {
        vm.periodOptions = CommonModel.periodOptions;
    });

    if (vm.filter.model.employeeId) {
        CommonModel.getEmployeeById(vm.filter.model.employeeId).then(function () {
            vm.filter.model.employee = CommonModel.employee;
            vm.filter.model.employee.Name = vm.filter.model.employee.NickName;
            vm.searchEmployee = vm.filter.model.employee.NickName + ' (' + vm.filter.model.employee.FullName + ')';
        });
    }

    EPAHistoryModel.checkHasOnlyEmployeeRole().then(function () {
        vm.onlyEmployeeRole = EPAHistoryModel.onlyEmployeeRole;
    });

    vm.getEmployee = function () {
        return CommonModel.getAllEmployee(vm.searchEmployee);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filter.model.employee) {
                vm.filter.model.employee = null;
            }
        }
    };

    var getEPAHistory = function (periodId, employeeId) {
        usSpinnerService.spin("spinner-1");

        EPAHistoryModel.getEPAHistory(periodId, employeeId).then(function () {
            vm.epas = EPAHistoryModel.epas;
            utilitiesFactory.searchOnPage(vm, vm.epas);
            usSpinnerService.stop('spinner-1');
        });
    };

    getEPAHistory(vm.filter.model.periodId, vm.filter.model.employeeId);

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