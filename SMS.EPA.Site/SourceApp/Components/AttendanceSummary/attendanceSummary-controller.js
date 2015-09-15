'use strict';
/*global attendanceModule*/

attendanceModule.controller('AttendanceSummaryController',
['AttendanceModel', 'utilitiesFactory', 'EmployeeModel', 'CommonModel', 'baseUrl',
function (AttendanceModel, utilitiesFactory, EmployeeModel, CommonModel, baseUrl) {
    var vm = this,
        date = new Date(),
        firstDate = new Date(date.getFullYear(), date.getMonth(), 1);

    vm.attendanceEmployees = AttendanceModel.attendanceEmployees;
    vm.violationOptions = AttendanceModel.violations;

    vm.currentPage = 1;
    vm.pageSize = 20;

    vm.filter = {
        startDate: firstDate,
        endDate: date,
        employee: null,
        departmentId: null,
        employeeId: null
    };

    vm.calendar = {
        startDateOpened: false,
        endDateOpened: false
    };

    vm.openPopupStartDate = function (event) {
        utilitiesFactory.openPopupStartDate(event, vm);
    };

    vm.openPopupEndDate = function (event) {
        utilitiesFactory.openPopupEndDate(event, vm);
    };

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    CommonModel.getDepartment().then(function () {
        vm.departments = CommonModel.departments;
        vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
    });

    AttendanceModel.getViolationType().then(function () {
        vm.violationOptions = AttendanceModel.violations;
    });

    var getAttendanceSummary = function (startDate, endDate, departmentId, employeeId) {
        AttendanceModel.getAttendanceSummary(startDate, endDate, departmentId, employeeId).then(function () {
            vm.attendanceEmployees = AttendanceModel.attendanceEmployees;
            utilitiesFactory.searchOnPage(vm, vm.attendanceEmployees);
        });
    };

    vm.getEmployee = function () {
        return EmployeeModel.getAllEmployee(vm.searchEmployee);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filter.employee) {
                vm.filter.employee = null;
            }
        }
    };

    getAttendanceSummary(vm.filter.startDate, vm.filter.endDate, vm.filter.departmentId, vm.filter.employeeId);

    vm.query = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        getAttendanceSummary(filter.startDate, filter.endDate, filter.departmentId, employeeId);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.addOtherViolations = function (employee) {
        AttendanceModel.openPopupOtherViolation(vm, employee, vm.filter.startDate, vm.filter.endDate).then(function () { }, function () {
            getAttendanceSummary(vm.filter.startDate, vm.filter.endDate, vm.filter.departmentId, vm.filter.employeeId);
        })['finally'](function () {
            vm.modalInstance = '';
        });
    };

    vm.adjust = function (employee) {
        vm.attendanceDetailUrl = baseUrl + 'Attendance/Detail/' + employee.EmployeeId;
    };
}]);

attendanceModule.controller("OtherViolationsController",
['$modalInstance', 'AttendanceModel', 'employee', 'startDate', 'endDate', 'utilitiesFactory', 'CommonModel',
function ($modalInstance, AttendanceModel, employee, startDate, endDate, utilitiesFactory, CommonModel) {
    var vm = this,
        date = new Date();

    vm.employee = employee;
    vm.startDate = startDate;
    vm.endDate = endDate;

    vm.otherViolations = AttendanceModel.otherViolations;

    vm.violation = {
        EmployeeId: vm.employee.EmployeeId,
        Name: null,
        Score: 0,
        WorkDate: date,
        Comment: null
    };

    vm.calendar = {
        startDateOpened: false,
        endDateOpened: false
    };

    vm.openPopupStartDate = function (event) {
        utilitiesFactory.openPopupStartDate(event, vm);
    };

    vm.currentPage = 1;
    vm.pageSize = 5;
    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    var getOtherViolations = function (employeeId, startDate, endDate) {
        AttendanceModel.getOtherViolations(employeeId, startDate, endDate).then(function () {
            vm.otherViolations = AttendanceModel.otherViolations;
            utilitiesFactory.searchOnPage(vm, vm.otherViolations);
        });
    };

    getOtherViolations(vm.employee.EmployeeId, vm.startDate, vm.endDate);

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.addViolation = function (violation) {
        if (AttendanceModel.validateOtherViolation(violation)) {
            AttendanceModel.insertOtherViolation(violation).then(function () {
                getOtherViolations(vm.employee.EmployeeId, vm.startDate, vm.endDate);
            });
        }
    };

    vm.delete = function (violation) {
        AttendanceModel.openPopupDeletePeriod(vm, violation);
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);

attendanceModule.controller('DetailAttendanceSummaryController',
['$window', 'AttendanceModel', 'utilitiesFactory', 'EmployeeModel', 'CommonModel',
function ($window, AttendanceModel, utilitiesFactory, EmployeeModel, CommonModel) {
    var vm = this,
        date = new Date(),
        firstDate = new Date(date.getFullYear(), date.getMonth(), 1);

    vm.currentPage = 1;
    vm.pageSize = 20;

    vm.violationOptions = AttendanceModel.violations;
    vm.attendanceDetails = AttendanceModel.attendanceDetails;

    vm.filter = {
        startDate: firstDate,
        endDate: date,
        employee: null,
        departmentId: null,
        employeeId: null,
        reasonId: null
    };

    vm.calendar = {
        startDateOpened: false,
        endDateOpened: false
    };

    vm.openPopupStartDate = function (event) {
        utilitiesFactory.openPopupStartDate(event, vm);
    };

    vm.openPopupEndDate = function (event) {
        utilitiesFactory.openPopupEndDate(event, vm);
    };

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    CommonModel.getDepartment().then(function () {
        vm.departments = CommonModel.departments;
        vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
    });

    AttendanceModel.getViolationType().then(function () {
        vm.violationOptions = AttendanceModel.violations;
    });

    var getAttendanceDetail = function (startDate, endDate, employeeId, departmentId, reasonId) {
        AttendanceModel.getAttendanceDetail(startDate, endDate, employeeId, departmentId, reasonId).then(function () {
            vm.attendanceDetails = AttendanceModel.attendanceDetails;
            utilitiesFactory.searchOnPage(vm, vm.attendanceDetails);
        });
    };

    vm.getEmployee = function () {
        return EmployeeModel.getAllEmployee(vm.searchEmployee);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filter.employee) {
                vm.filter.employee = null;
            }
        }
    };

    getAttendanceDetail(vm.filter.startDate, vm.filter.endDate, vm.filter.employeeId, vm.filter.departmentId, vm.filter.reasonId);

    vm.query = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        getAttendanceDetail(vm.filter.startDate, vm.filter.endDate, employeeId, vm.filter.departmentId, vm.filter.reasonId);
    };

    vm.backToList = function () {
        $window.history.back();
    };
}]);