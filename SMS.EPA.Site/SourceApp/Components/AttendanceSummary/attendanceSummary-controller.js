'use strict';
/*global attendanceModule*/

attendanceModule.controller('AttendanceSummaryController',
['AttendanceModel', 'utilitiesFactory', 'EmployeeModel', 'CommonModel',
function (AttendanceModel, utilitiesFactory, EmployeeModel, CommonModel) {
    var vm = this,
        date = new Date();

    vm.attendanceEmployees = AttendanceModel.attendanceEmployees;
    vm.currentPage = 1;
    vm.pageSize = 20;

    vm.filter = {
        startDate: date,
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
}]);