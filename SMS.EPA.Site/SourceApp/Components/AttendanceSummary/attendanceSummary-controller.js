'use strict';
/*global attendanceModule*/

attendanceModule.controller('AttendanceSummaryController',
['$scope', 'AttendanceModel', 'utilitiesFactory', 'CommonModel', 'enumerationsFactory', 'baseUrl', 'lockScreenService', 'historyFactory',
function ($scope, AttendanceModel, utilitiesFactory, CommonModel, enumerationsFactory, baseUrl, lockScreenService, historyFactory) {
    var vm = this;

    vm.attendanceEmployees = [];
    vm.violationOptions = [];
    vm.permission = AttendanceModel.permission;
    vm.policyEnum = enumerationsFactory.policy;
    vm.currentPage = AttendanceModel.summaryPaging.currentPage;
    vm.pageSize = AttendanceModel.summaryPaging.pageSize;

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            startDate: AttendanceModel.getStartDate(),
            endDate: AttendanceModel.getEndDate(),
            employee: null,
            departmentId: null,
            isLocal: '',
            employeeId: null,
            reasonId: null
        };
    }

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

    vm.getEmployee = function () {
        return CommonModel.getAllEmployee(vm.searchEmployee, null, null, null, true, true);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filter.employee) {
                vm.filter.employee = null;
                vm.filter.employeeId = null;
            }
        }
    };

    function getAttendanceSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId) {
        lockScreenService.start();

        AttendanceModel.getAttendanceSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId)
        .then(function () {
            vm.attendanceEmployees = AttendanceModel.attendanceEmployees;
            utilitiesFactory.searchOnPage(vm, vm.attendanceEmployees);
            lockScreenService.stop();
        });

        historyFactory.saveState(vm.filter);
    }

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

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.viewOtherViolations = function (employee) {
        AttendanceModel.openPopupOtherViolation(vm, employee, vm.filter.startDate, vm.filter.endDate)
        .then()['finally'](function () {
            vm.modalInstance = '';
        });
    };

    vm.viewDetail = function (employee) {
        var startDate = vm.filter.startDate,
            endDate = vm.filter.endDate;

        AttendanceModel.setSummaryPaging(vm.currentPage, vm.pageSize);

        vm.attendanceDetailUrl = baseUrl + 'Discipline/Detail/' + employee.Employee.EmployeeId
            + '?startDate=' + startDate.toDateString() + '&endDate=' + endDate.toDateString() + '&reasonId='
            + vm.filter.reasonId + '&isLocal=' + vm.filter.isLocal;
    };

    vm.exportReport = function (filter) {
        vm.exportCriteria = angular.copy(filter);
        vm.exportCriteria.employeeName = filter.employee ? filter.employee.Name : '';
        vm.exportCriteria.departmentName = filter.departmentId ? AttendanceModel.getDepartmentName(filter.departmentId, vm.departments) : '';
        vm.exportCriteria.isLocal = filter.isLocal;
        vm.exportCriteria.startDate = filter.startDate.toDateString();
        vm.exportCriteria.endDate = filter.endDate.toDateString();
        $scope.$broadcast('exportAttendanceSummaryReport');
    };

    function initialize() {
        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        AttendanceModel.getViolationType(false, true).then(function () {
            vm.violationOptions = AttendanceModel.violationTypes;
        });

        AttendanceModel.getUserPermissionOnAttendanceSummary().then(function () {
            vm.permission = AttendanceModel.permission;
        });

        vm.filter = initFilter();

        if (vm.filter.employeeId) {
            CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
                vm.filter.employee = CommonModel.employee;
                vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
            });
        }

        getAttendanceSummary(vm.filter.startDate, vm.filter.endDate, vm.filter.departmentId, vm.filter.isLocal, vm.filter.employeeId, vm.filter.reasonId);
    }

    initialize();
}]);

attendanceModule.controller("OtherViolationsController",
['$scope', '$modalInstance', 'AttendanceModel', 'utilitiesFactory', 'employee', 'startDate', 'endDate',
function ($scope, $modalInstance, AttendanceModel, utilitiesFactory, employee, startDate, endDate) {
    var vm = this;

    vm.employee = employee;
    vm.startDate = startDate;
    vm.endDate = endDate;
    vm.otherViolations = [];

    utilitiesFactory.getDateFormat(vm);

    function getOtherViolations(employeeId, startDate, endDate) {
        AttendanceModel.getOtherViolations(employeeId, startDate, endDate)
        .then(function () {
            vm.otherViolations = AttendanceModel.otherViolations;
        });
    }

    if (vm.employee) {
        getOtherViolations(vm.employee.Employee.EmployeeId, vm.startDate, vm.endDate);
    }

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);

attendanceModule.controller('DetailAttendanceSummaryController',
['$scope', '$window', '$routeParams', 'AttendanceModel', 'CommonModel', 'utilitiesFactory', 'enumerationsFactory', 'lockScreenService', 'historyFactory',
function ($scope, $window, $routeParams, AttendanceModel, CommonModel, utilitiesFactory, enumerationsFactory, lockScreenService, historyFactory) {
    var vm = this;

    vm.currentPage = 1;
    vm.pageSize = 15;
    vm.violationOptions = [];
    vm.attendanceDetails = [];

    vm.fromSummaryPage = $routeParams.id ? true : false;

    vm.filter = {
        startDate: $routeParams.startDate ? new Date($routeParams.startDate) : AttendanceModel.getStartDate(),
        endDate: $routeParams.endDate ? new Date($routeParams.endDate) : AttendanceModel.getEndDate(),
        employee: null,
        departmentId: null,
        isLocal: $routeParams.isLocal ? $routeParams.isLocal : '',
        employeeId: $routeParams.id ? parseInt($routeParams.id) : null,
        reasonId: parseInt($routeParams.reasonId),
        hasIgnored: ''
    };

    if (vm.filter.employeeId) {
        CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
            vm.filter.employee = CommonModel.employee;
            vm.filter.employee.Name = vm.filter.employee.NickName;
            vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
        });
    }

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
    utilitiesFactory.getTimeFormat(vm);

    vm.getEmployee = function () {
        return CommonModel.getAllEmployee(vm.searchEmployee, null, null, null, true, true);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filter.employee) {
                vm.filter.employee = null;
                vm.filter.employeeId = null;
            }
        }
    };

    function getAttendanceDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored) {
        lockScreenService.start();

        AttendanceModel.getAttendanceDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored)
        .then(function () {
            vm.attendanceDetails = AttendanceModel.attendanceDetails;
            utilitiesFactory.searchOnPage(vm, vm.attendanceDetails);
            lockScreenService.stop();
        });
    }

    vm.query = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        getAttendanceDetail(
            vm.filter.startDate,
            vm.filter.endDate,
            employeeId,
            vm.filter.departmentId,
            vm.filter.isLocal,
            vm.filter.reasonId,
            vm.filter.hasIgnored);
    };

    vm.backToList = function () {
        historyFactory.back();
    };

    vm.exportReport = function (filter) {
        vm.exportCriteria = angular.copy(filter);
        vm.exportCriteria.employeeName = filter.employee ? filter.employee.Name : '';
        vm.exportCriteria.departmentName = filter.departmentId ? AttendanceModel.getDepartmentName(filter.departmentId, vm.departments) : '';
        vm.exportCriteria.isLocal = filter.isLocal;
        vm.exportCriteria.startDate = filter.startDate.toDateString();
        vm.exportCriteria.endDate = filter.endDate.toDateString();
        $scope.$broadcast('exportAttendanceDetailReport');
    };

    function initialize() {
        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        AttendanceModel.getViolationType(false, true).then(function () {
            vm.violationOptions = AttendanceModel.violationTypes;
        });

        getAttendanceDetail(
            vm.filter.startDate,
            vm.filter.endDate,
            vm.filter.employeeId,
            vm.filter.departmentId,
            vm.filter.isLocal,
            vm.filter.reasonId,
            vm.filter.hasIgnored);
    }

    initialize();
}]);