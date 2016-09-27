(function (angular) {
    'use strict';

    angular.module('disciplineModule').controller('AttendanceSummaryController', attendanceSummaryController);
    attendanceSummaryController.$inject = ['$scope', 'DisciplineSummaryModel', 'CommonModel', 'utilitiesFactory', 'baseUrl', 'lockScreenService', 'historyFactory'];

    function attendanceSummaryController($scope, DisciplineSummaryModel, CommonModel, utilitiesFactory, baseUrl, lockScreenService, historyFactory) {
        var vm = this;

        vm.employeeDisciplines = [];
        vm.calendar = {
            startDateOpened: false,
            endDateOpened: false
        };
        vm.currentPage = DisciplineSummaryModel.summaryPaging.currentPage;
        vm.pageSize = DisciplineSummaryModel.summaryPaging.pageSize;
        vm.permission = DisciplineSummaryModel.permission;
        vm.policyEnum = utilitiesFactory.policy;
        vm.disciplineTypes = [];

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

            DisciplineSummaryModel.getDisciplineType(true, true).then(function () {
                vm.disciplineTypes = DisciplineSummaryModel.disciplineTypes;
            });

            DisciplineSummaryModel.getUserPermissionOnDisciplineSummary().then(function () {
                vm.permission = DisciplineSummaryModel.permission;
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
            vm.exportCriteria = angular.copy(filter);
            vm.exportCriteria.employeeName = filter.employee ? filter.employee.Name : '';
            vm.exportCriteria.departmentName = filter.departmentId ? DisciplineSummaryModel.getDepartmentName(filter.departmentId, vm.departments) : '';
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

        vm.viewDetail = function (employee) {
            var startDate = vm.filter.startDate,
                endDate = vm.filter.endDate;

            DisciplineSummaryModel.setSummaryPaging(vm.currentPage, vm.pageSize);

            vm.attendanceDetailUrl = baseUrl + 'Discipline/Detail/' + employee.Employee.EmployeeId
                + '?startDate=' + startDate.toDateString() + '&endDate=' + endDate.toDateString() + '&reasonId='
                + vm.filter.reasonId + '&isLocal=' + vm.filter.isLocal;
        };

        vm.viewOtherViolations = function (employee) {
            DisciplineSummaryModel.openPopupOtherViolation(vm, employee, vm.filter.startDate, vm.filter.endDate).then()['finally'](function () {
                vm.modalInstance = '';
            });
        };
    }
})(angular);