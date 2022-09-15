(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('disciplineModule').controller('DisciplineSummaryForOtherSystemController', disciplineSummaryForOtherSystemController);
    disciplineSummaryForOtherSystemController.$inject = ['$scope', 'DisciplineSummaryModel', 'CommonModel', 'lockScreenService',
        'utilitiesFactory', 'historyFactory'];

    function disciplineSummaryForOtherSystemController($scope, DisciplineSummaryModel, CommonModel, lockScreenService, utilitiesFactory, historyFactory) {
        let vm = this;

        vm.orgChart = [];
        vm.employeeDisciplines = [];
        vm.calendar = {
            startDateOpened: false,
            endDateOpened: false
        };
        vm.currentPage = 1;
        vm.disciplineTypes = [
            { Id: 1, Name: 'Change Shift' },
            { Id: 2, Name: 'Missing Check In/Out' },
            { Id: 3, Name: 'Late In' },
            { Id: 4, Name: 'Early Out' },
            { Id: 5, Name: 'Absent' },
            { Id: 6, Name: 'Unpaid Leave' },
            { Id: 7, Name: 'Working Undertime' },
            { Id: 8, Name: 'Unpaid Leave Absent' },
            { Id: 9, Name: 'Soccer Leave Policy' },
            { Id: 10, Name: 'Weekend Leave' },
            { Id: 11, Name: 'Seven days leave' },
            { Id: -1, Name: 'Other Discipline' }
        ];
        vm.pageSize = 20;

        vm.exportReport = exportReport;
        vm.getEmployee = getEmployee;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.unSelectEmployee = unSelectEmployee;

        initialize();

        function exportReport(filter) {
            vm.exportCriteria = angular.copy(filter);
            vm.exportCriteria.isLocal = filter.isLocal;
            vm.exportCriteria.startDate = filter.startDate.toDateString();
            vm.exportCriteria.endDate = filter.endDate.toDateString();
            $scope.$broadcast('exportDisciplineSummaryForOtherSystemReport');
        }

        function getDisciplineSummary(startDate, endDate, teamIds, isLocal, employeeId, reasonId) {
            lockScreenService.start();

            DisciplineSummaryModel.getDisciplineSummaryForOtherSystem(startDate, endDate, teamIds, isLocal, employeeId, reasonId).then(function () {
                vm.employeeDisciplines = DisciplineSummaryModel.employeeDisciplines;
                utilitiesFactory.searchOnPage(vm, vm.employeeDisciplines);
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        }

        function getEmployee() {
            return CommonModel.searchEmployee(vm.searchKeyword, vm.filter.teamIds, null, null, true, true);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function initFilter() {
            var filter = historyFactory.getState(),
                dateRangeFilter = DisciplineSummaryModel.getRangeDateFilter();

            vm.filter = filter ? filter : {
                startDate: dateRangeFilter.startDate,
                endDate: dateRangeFilter.endDate,
                employee: null,
                teamIds: null,
                isLocal: '',
                employeeId: null,
                reasonId: null
            };

            if (vm.filter.employeeId) {
                CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
                    vm.filter.employee = CommonModel.employee;
                    vm.searchKeyword = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
                });
            }
        }

        function initialize() {
            utilitiesFactory.numberEntryList(vm);
            utilitiesFactory.getDateFormat(vm);
            initFilter();

            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            getDisciplineSummary(
                vm.filter.startDate,
                vm.filter.endDate,
                vm.filter.departmentId,
                vm.filter.isLocal,
                vm.filter.employeeId,
                vm.filter.reasonId);
        }

        function query(filter) {
            let employeeId = null;
            vm.searchKey = '';
            vm.currentPage = 1;
            vm.pageSize = 20;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
                vm.filter.employeeId = employeeId;
            }

            getDisciplineSummary(filter.startDate, filter.endDate, filter.teamIds, filter.isLocal, employeeId, filter.reasonId);
        }

        function unSelectEmployee() {
            if (vm.searchKeyword.trim() === '' && vm.filter.employee) {
                vm.filter.employee = null;
                vm.filter.employeeId = null;
            }
        }

        vm.openPopupEndDate = function (event) {
            utilitiesFactory.openPopupEndDate(event, vm);
        };

        vm.openPopupStartDate = function (event) {
            utilitiesFactory.openPopupStartDate(event, vm);
        };

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };
    }
})(angular);