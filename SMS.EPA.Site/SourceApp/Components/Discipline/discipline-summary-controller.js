(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('disciplineModule').controller('DisciplineSummaryController', disciplineSummaryController);
    disciplineSummaryController.$inject = ['$scope', 'DisciplineSummaryModel', 'CommonModel',
        'utilitiesFactory', 'lockScreenService', 'historyFactory', 'baseUrl'];

    function disciplineSummaryController($scope, DisciplineSummaryModel, CommonModel, utilitiesFactory, lockScreenService, historyFactory, baseUrl) {
        var vm = this;

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
            { Id: 9, Name: 'Not Following Leave Procedures' },
            { Id: 10, Name: 'Weekend Leave' },
            { Id: -1, Name: 'Other Violations' }
        ];

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);

        function getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId, isShowAll) {
            lockScreenService.start();

            DisciplineSummaryModel.getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId, isShowAll).then(function () {
                vm.employeeDisciplines = DisciplineSummaryModel.employeeDisciplines;
                utilitiesFactory.searchOnPage(vm, vm.employeeDisciplines);
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
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
                reasonId: null,
                isShowAll: false
            };

            if (vm.filter.employeeId) {
                CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
                    vm.filter.employee = CommonModel.employee;
                    vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
                });
            }
        }

        function initialize() {
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
                vm.filter.reasonId,
                vm.filter.isShowAll);
        }

        initialize();

        vm.exportReport = function (filter) {
            var index = vm.departments.map(function (e) { return e.DepartmentId; }).indexOf(filter.departmentId);

            vm.exportCriteria = angular.copy(filter);
            vm.exportCriteria.employeeName = filter.employee ? filter.employee.Name : '';
            vm.exportCriteria.departmentName = index !== -1 ? vm.departments[index].Name : '';
            vm.exportCriteria.isLocal = filter.isLocal;
            vm.exportCriteria.startDate = filter.startDate.toDateString();
            vm.exportCriteria.endDate = filter.endDate.toDateString();
            $scope.$broadcast('exportDisciplineSummaryReport');
        };

        vm.getEmployee = function () {
            return CommonModel.getAllEmployee(vm.searchEmployee, null, null, null, true, true);
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

        vm.query = function (filter) {
            var employeeId = null;
            vm.searchKey = '';

            DisciplineSummaryModel.resetSummaryPaging();
            vm.currentPage = DisciplineSummaryModel.summaryPaging.currentPage;
            vm.pageSize = DisciplineSummaryModel.summaryPaging.pageSize;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
                vm.filter.employeeId = employeeId;
            }

            getDisciplineSummary(filter.startDate, filter.endDate, filter.departmentId, filter.isLocal, employeeId, filter.reasonId, filter.isShowAll);
        };

        vm.unSelectEmployee = function () {
            if (vm.searchEmployee.trim() === '') {
                if (vm.filter.employee) {
                    vm.filter.employee = null;
                    vm.filter.employeeId = null;
                }
            }
        };
    }
})(angular);