(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaHistoryModule').controller('MonthlyEPAHistoryController', monthlyEPAHistoryController);
    monthlyEPAHistoryController.$inject = ['$routeParams', 'EPAHistoryModel', 'CommonModel',
        'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl'];

    function monthlyEPAHistoryController($routeParams, EPAHistoryModel,
        CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.currentPage = 1;
        vm.epas = [];
        vm.onlyEmployeeRole = EPAHistoryModel.onlyEmployeeRole;
        vm.pageSize = 20;
        vm.periodOptions = [];
        vm.treeDepartment = null;

        vm.backToList = backToList;
        vm.getEmployee = getEmployee;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.unSelectEmployee = unSelectEmployee;
        vm.viewDetailEPA = viewDetailEPA;

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);
        initialize();

        function backToList() {
            historyFactory.back();
        }

        function getEmployee() {
            return CommonModel.getAllEmployee(vm.searchEmployee);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getMonthlyEPAHistory(periodId, employeeId) {
            lockScreenService.start();

            EPAHistoryModel.getMonthlyEPAHistory(periodId, employeeId).then(function () {
                vm.epas = EPAHistoryModel.epas;
                utilitiesFactory.searchOnPage(vm, vm.epas);
                lockScreenService.stop();
            });
        }

        function initFilter() {
            vm.filter = {
                periodId: $routeParams.periodId ? $routeParams.periodId : null,
                employee: null,
                employeeId: $routeParams.empId ? $routeParams.empId : null
            };
        }

        function initialize() {
            initFilter();

            EPAHistoryModel.checkHasOnlyEmployeeRole().then(function () {
                vm.onlyEmployeeRole = EPAHistoryModel.onlyEmployeeRole;
            });

            CommonModel.getDepartment().then(function () {
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            });

            if (vm.filter.employeeId) {
                vm.fromEPAListing = true;

                CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
                    vm.filter.employee = CommonModel.employee;
                    vm.filter.employee.Name = vm.filter.employee.NickName;
                    vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
                });
            }

            CommonModel.getAllMonthlyPeriod().then(function () {
                vm.periodOptions = CommonModel.monthlyPeriods;

                if (vm.periodOptions.length > 0 && !$routeParams.periodId) {
                    vm.filter.periodId = vm.periodOptions[0].PeriodId;
                }

                getMonthlyEPAHistory(vm.filter.periodId, vm.filter.employeeId);
            });
        }

        function query() {
            vm.currentPage = 1;
            var employeeId = null;

            if (vm.filter.employee) {
                employeeId = vm.filter.employee.EmployeeId;
            }

            getMonthlyEPAHistory(vm.filter.periodId, employeeId);
        }

        function unSelectEmployee() {
            if (vm.searchEmployee.trim() === '') {
                if (vm.filter.employee) {
                    vm.filter.employee = null;
                }
            }
        }

        function viewDetailEPA(epaId) {
            vm.viewEPADetailUrl = baseUrl + 'EPA/Detail/' + epaId;
        }
    }
})(angular);