(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaHistoryModule').controller('EPAHistoryController', epaHistoryController);
    epaHistoryController.$inject = ['$routeParams', '$location', 'EPAHistoryModel', 'CommonModel', 'utilitiesFactory',
        'historyFactory', 'lockScreenService', 'baseUrl'];

    function epaHistoryController($routeParams, $location, EPAHistoryModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.currentPage = 1;
        vm.epas = [];
        vm.onlyEmployeeRole = false;
        vm.pageSize = 20;
        vm.periodOptions = [];
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.orgChart = [];

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
            return CommonModel.searchEmployee(vm.searchKeyword, vm.filter.teamIds);
        }

        function getEPAHistory(periodId, teamIds, isLocal, employeeId) {
            lockScreenService.start();

            EPAHistoryModel.getEPAHistory(periodId, teamIds, isLocal, employeeId).then(function () {
                vm.epas = EPAHistoryModel.epas;
                utilitiesFactory.searchOnPage(vm, vm.epas);
                lockScreenService.stop();
            });
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function initFilter() {
            vm.filter = {
                periodId: $routeParams.periodId ? $routeParams.periodId : null,
                teamIds: '',
                isLocal: '',
                employee: null,
                employeeId: $routeParams.empId ? $routeParams.empId : null
            };
        }

        function initialize() {
            initFilter();

            EPAHistoryModel.checkHasOnlyEmployeeRole().then(function () {
                vm.onlyEmployeeRole = EPAHistoryModel.onlyEmployeeRole;
            });

            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            if (vm.filter.employeeId) {
                vm.fromEPAListing = true;

                CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
                    vm.filter.employee = CommonModel.employee;
                    vm.filter.employee.Name = vm.filter.employee.NickName;
                    vm.searchKeyword = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
                });
            }

            CommonModel.getAllPeriodIncludeImport().then(function () {
                vm.periodOptions = CommonModel.periodOptions;

                if (vm.periodOptions.length > 0 && !$routeParams.periodId) {
                    if (vm.periodOptions.length > 1) {
                        /* Select last period */
                        vm.filter.periodId = vm.periodOptions[1].PeriodId;
                    } else {
                        /* Select latest period since it's the only option */
                        vm.filter.periodId = vm.periodOptions[0].PeriodId;
                    }
                }

                getEPAHistory(vm.filter.periodId, vm.filter.teamIds, vm.filter.isLocal, vm.filter.employeeId);
            });
        }

        function query(filter) {
            vm.currentPage = 1;
            var employeeId = null;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
            }

            getEPAHistory(filter.periodId, filter.teamIds, filter.isLocal, employeeId);
        }

        function unSelectEmployee() {
            if (vm.searchKeyword.trim() === '') {
                if (vm.filter.employee) {
                    vm.filter.employee = null;
                }
            }
        }

        function viewDetailEPA(epa) {
            $location.path(baseUrl + 'EPA/Detail/' + epa.EPAId);
        }

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };
    }
})(angular);