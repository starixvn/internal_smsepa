(function (angular) {
    'use strict';

    angular.module('epaHistoryModule').controller('EPAHistoryController', epaHistoryController);
    epaHistoryController.$inject = ['$routeParams', 'EPAHistoryModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl'];

    function epaHistoryController($routeParams, EPAHistoryModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.currentPage = 1;
        vm.epas = [];
        vm.filter = {
            periodId: $routeParams.periodId ? $routeParams.periodId : null,
            departmentId: '',
            isLocal: '',
            employee: null,
            employeeId: $routeParams.empId ? $routeParams.empId : null
        };
        vm.onlyEmployeeRole = false;
        vm.pageSize = 20;
        vm.periodOptions = [];
        vm.targetTypeEnum = utilitiesFactory.targetType;
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

        function getEPAHistory(periodId, departmentId, isLocal, employeeId) {
            lockScreenService.start();

            EPAHistoryModel.getEPAHistory(periodId, departmentId, isLocal, employeeId).then(function () {
                vm.epas = EPAHistoryModel.epas;
                utilitiesFactory.searchOnPage(vm, vm.epas);
                lockScreenService.stop();
            });
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function initialize() {
            CommonModel.getAllPeriodIncludeImport().then(function () {
                vm.periodOptions = CommonModel.periodOptions;
            });

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

            getEPAHistory(vm.filter.periodId, vm.filter.departmentId, vm.filter.isLocal, vm.filter.employeeId);
        }

        function query(filter) {
            vm.currentPage = 1;
            var employeeId = null;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
            }

            getEPAHistory(filter.periodId, filter.departmentId, filter.isLocal, employeeId);
        }

        function unSelectEmployee() {
            if (vm.searchEmployee.trim() === '') {
                if (vm.filter.employee) {
                    vm.filter.employee = null;
                }
            }
        }

        function viewDetailEPA(epa) {
            if (epa.HasMonthlyEPA && epa.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
                vm.viewEPADetailUrl = baseUrl + 'EPA/SemiDetail/' + epa.EPAId;
            } else {
                vm.viewEPADetailUrl = baseUrl + 'EPA/Detail/' + epa.EPAId;
            }
        }
    }
})(angular);