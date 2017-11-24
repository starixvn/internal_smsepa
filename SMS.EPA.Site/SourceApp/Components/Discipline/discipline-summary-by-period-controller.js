(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('disciplineModule').controller('DisciplineSummaryForOtherSystemController', disciplineSummaryForOtherSystemController);
    disciplineSummaryForOtherSystemController.$inject = ['$scope', 'DisciplineSummaryModel', 'CommonModel', 'lockScreenService',
        'utilitiesFactory', 'historyFactory'];

    function disciplineSummaryForOtherSystemController($scope, DisciplineSummaryModel, CommonModel, lockScreenService,
        utilitiesFactory, historyFactory) {
        var vm = this;

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
            { Id: 9, Name: 'Not Following Leave Procedures' },
            { Id: 10, Name: 'Weekend Leave' },
            { Id: 11, Name: 'Seven days leave' },
            { Id: -1, Name: 'Other Discipline' }
        ];
        vm.employeeDisciplines = [];
        vm.pageSize = 20;

        vm.exportReport = exportReport;
        vm.getEmployee = getEmployee;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.unSelectEmployee = unSelectEmployee;

        initialize();

        function exportReport(filter) {
            var index = vm.departments.map(function (e) { return e.DepartmentId; }).indexOf(filter.departmentId);

            vm.exportCriteria = angular.copy(filter);
            vm.exportCriteria.employeeName = filter.employee ? filter.employee.Name : '';
            vm.exportCriteria.departmentName = index !== -1 ? vm.departments[index].Name : '';
            vm.exportCriteria.isLocal = filter.isLocal;
            vm.exportCriteria.startDate = filter.startDate.toDateString();
            vm.exportCriteria.endDate = filter.endDate.toDateString();
            $scope.$broadcast('exportDisciplineSummaryForOtherSystemReport');
        }

        function getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId) {
            lockScreenService.start();

            DisciplineSummaryModel.getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId).then(function () {
                vm.employeeDisciplines = DisciplineSummaryModel.employeeDisciplines;
                utilitiesFactory.searchOnPage(vm, vm.employeeDisciplines);
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        }

        function getEmployee() {
            return CommonModel.getAllEmployee(vm.searchEmployee, null, null, null, true, true);
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
                departmentId: null,
                isLocal: '',
                employeeId: null,
                reasonId: null
            };

            if (vm.filter.employeeId) {
                CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
                    vm.filter.employee = CommonModel.employee;
                    vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
                });
            }
        }

        function initialize() {
            utilitiesFactory.numberEntryList(vm);
            utilitiesFactory.getDateFormat(vm);
            initFilter();

            CommonModel.getDepartment().then(function () {
                vm.departments = CommonModel.departments;
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
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
            var employeeId = null;
            vm.searchKey = '';
            vm.currentPage = 1;
            vm.pageSize = 20;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
                vm.filter.employeeId = employeeId;
            }

            getDisciplineSummary(filter.startDate, filter.endDate, filter.departmentId, filter.isLocal, employeeId, filter.reasonId);
        }

        function unSelectEmployee() {
            if (vm.searchEmployee.trim() === '' && vm.filter.employee) {
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
    }
})(angular);