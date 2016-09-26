(function (angular) {
    'use strict';

    angular.module('attendanceModule').controller('AttendanceSummaryController', attendanceSummaryController);
    attendanceSummaryController.$inject = ['$scope', 'AttendanceModel', 'CommonModel', 'utilitiesFactory', 'baseUrl', 'lockScreenService', 'historyFactory'];

    function attendanceSummaryController($scope, AttendanceModel, CommonModel, utilitiesFactory, baseUrl, lockScreenService, historyFactory) {
        var vm = this;

        vm.attendanceEmployees = [];
        vm.calendar = {
            startDateOpened: false,
            endDateOpened: false
        };
        vm.currentPage = AttendanceModel.summaryPaging.currentPage;
        vm.pageSize = AttendanceModel.summaryPaging.pageSize;
        vm.permission = AttendanceModel.permission;
        vm.policyEnum = utilitiesFactory.policy;
        vm.violationOptions = [];

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.getDateFormat(vm);

        function getAttendanceSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId) {
            lockScreenService.start();

            AttendanceModel.getAttendanceSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId).then(function () {
                vm.attendanceEmployees = AttendanceModel.attendanceEmployees;
                utilitiesFactory.searchOnPage(vm, vm.attendanceEmployees);
                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        }

        function initFilter() {
            var filter = historyFactory.getState(),
                dateRangeFilter = AttendanceModel.getRangeDateFilter();

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
            initFilter();

            CommonModel.getDepartment().then(function () {
                vm.departments = CommonModel.departments;
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            });

            AttendanceModel.getViolationType(true, true).then(function () {
                vm.violationOptions = AttendanceModel.violationTypes;
            });

            AttendanceModel.getUserPermissionOnAttendanceSummary().then(function () {
                vm.permission = AttendanceModel.permission;
            });

            getAttendanceSummary(vm.filter.startDate, vm.filter.endDate, vm.filter.departmentId, vm.filter.isLocal, vm.filter.employeeId, vm.filter.reasonId);
        }

        initialize();

        vm.exportReport = function (filter) {
            vm.exportCriteria = angular.copy(filter);
            vm.exportCriteria.employeeName = filter.employee ? filter.employee.Name : '';
            vm.exportCriteria.departmentName = filter.departmentId ? AttendanceModel.getDepartmentName(filter.departmentId, vm.departments) : '';
            vm.exportCriteria.isLocal = filter.isLocal;
            vm.exportCriteria.startDate = filter.startDate.toDateString();
            vm.exportCriteria.endDate = filter.endDate.toDateString();
            $scope.$broadcast('exportAttendanceSummaryReport');
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

            AttendanceModel.resetSummaryPaging();
            vm.currentPage = AttendanceModel.summaryPaging.currentPage;
            vm.pageSize = AttendanceModel.summaryPaging.pageSize;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
                vm.filter.employeeId = employeeId;
            }

            getAttendanceSummary(filter.startDate, filter.endDate, filter.departmentId, filter.isLocal, employeeId, vm.filter.reasonId);
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

            AttendanceModel.setSummaryPaging(vm.currentPage, vm.pageSize);

            vm.attendanceDetailUrl = baseUrl + 'Discipline/Detail/' + employee.Employee.EmployeeId
                + '?startDate=' + startDate.toDateString() + '&endDate=' + endDate.toDateString() + '&reasonId='
                + vm.filter.reasonId + '&isLocal=' + vm.filter.isLocal;
        };

        vm.viewOtherViolations = function (employee) {
            AttendanceModel.openPopupOtherViolation(vm, employee, vm.filter.startDate, vm.filter.endDate)
            .then()['finally'](function () {
                vm.modalInstance = '';
            });
        };
    }
})(angular);