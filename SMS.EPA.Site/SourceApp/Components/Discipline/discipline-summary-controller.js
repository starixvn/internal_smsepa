(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('disciplineModule').controller('DisciplineSummaryController', disciplineSummaryController);
    disciplineSummaryController.$inject = ['$scope', 'DisciplineSummaryModel', 'CommonModel',
        'utilitiesFactory', 'lockScreenService', 'historyFactory', 'baseUrl'];

    function disciplineSummaryController($scope, DisciplineSummaryModel, CommonModel, utilitiesFactory, lockScreenService, historyFactory, baseUrl) {
        let vm = this;

        vm.orgChart = [];
        vm.employeeDisciplines = [];
        vm.calendar = {
            startDateOpened: false,
            endDateOpened: false
        };
        vm.currentPage = DisciplineSummaryModel.summaryPaging.currentPage;
        vm.pageSize = DisciplineSummaryModel.summaryPaging.pageSize;
        vm.policyEnum = utilitiesFactory.policy;
        vm.disciplineTypes = [
            { Id: 1, Name: 'Change Shift' },
            { Id: 2, Name: 'Missing Check In/Out' },
            { Id: 3, Name: 'Late In' },
            { Id: 4, Name: 'Early Out' },
            { Id: 5, Name: 'Absent' },
            { Id: 6, Name: 'Unpaid Leave' },
            { Id: 7, Name: 'Working Under Time' },
            { Id: 8, Name: 'Unpaid Leave Absent' },
            { Id: 9, Name: 'Late Leave Request' },
            { Id: 10, Name: 'Weekend Leave' },
            { Id: 11, Name: 'Seven Days Leave' },
            { Id: -1, Name: 'Other Violations' }
        ];

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);

        function getDisciplineSummary(startDate, endDate, teamIds, isLocal, employeeId, reasonId, isShowAll) {
            lockScreenService.start();

            DisciplineSummaryModel.getDisciplineSummary(startDate, endDate, teamIds, isLocal, employeeId, reasonId, isShowAll).then(function () {
                vm.employeeDisciplines = DisciplineSummaryModel.employeeDisciplines;
                utilitiesFactory.searchOnPage(vm, vm.employeeDisciplines);
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        }

        function initFilter() {
            let filter = historyFactory.getState(),
                dateRangeFilter = DisciplineSummaryModel.getRangeDateFilter();

            vm.filter = filter ? filter : {
                startDate: dateRangeFilter.startDate,
                endDate: dateRangeFilter.endDate,
                employee: null,
                teamIds: null,
                isLocal: '',
                employeeId: null,
                reasonId: null,
                isShowAll: false
            };

            if (vm.filter.employeeId) {
                CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
                    vm.filter.employee = CommonModel.employee;
                    vm.searchKeyword = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
                });
            }
        }

        function initialize() {
            initFilter();

            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            getDisciplineSummary(
                vm.filter.startDate,
                vm.filter.endDate,
                vm.filter.teamIds,
                vm.filter.isLocal,
                vm.filter.employeeId,
                vm.filter.reasonId,
                vm.filter.isShowAll);
        }

        initialize();

        vm.exportReport = function (filter) {
            vm.exportCriteria = angular.copy(filter);
            vm.exportCriteria.isLocal = filter.isLocal;
            vm.exportCriteria.startDate = filter.startDate.toDateString();
            vm.exportCriteria.endDate = filter.endDate.toDateString();
            $scope.$broadcast('exportDisciplineSummaryReport');
        };

        vm.getEmployee = function () {
            return CommonModel.searchEmployee(vm.searchKeyword, vm.filter.teamIds, null, null, true, true);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.openPopupEndDate = function (event) {
            utilitiesFactory.openPopupEndDate(event, vm);
        };

        vm.openPopupStartDate = function (event) {
            utilitiesFactory.openPopupStartDate(event, vm);
        };

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };

        vm.query = function (filter) {
            let employeeId = null;
            vm.searchKey = '';

            DisciplineSummaryModel.resetSummaryPaging();
            vm.currentPage = DisciplineSummaryModel.summaryPaging.currentPage;
            vm.pageSize = DisciplineSummaryModel.summaryPaging.pageSize;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
                vm.filter.employeeId = employeeId;
            }

            getDisciplineSummary(filter.startDate, filter.endDate, filter.teamIds, filter.isLocal, employeeId, filter.reasonId, filter.isShowAll);
        };

        vm.unSelectEmployee = function () {
            if (vm.searchKeyword.trim() === '') {
                if (vm.filter.employee) {
                    vm.filter.employee = null;
                    vm.filter.employeeId = null;
                }
            }
        };
    }
})(angular);