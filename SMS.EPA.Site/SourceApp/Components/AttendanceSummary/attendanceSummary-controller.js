'use strict';
/*global attendanceModule*/

attendanceModule.controller('AttendanceSummaryController',
['$scope', 'AttendanceModel', 'utilitiesFactory', 'CommonModel', 'baseUrl',
function ($scope, AttendanceModel, utilitiesFactory, CommonModel, baseUrl) {
    var vm = this;

    vm.attendanceEmployees = AttendanceModel.attendanceEmployees;
    vm.violationOptions = AttendanceModel.violations;
    vm.permission = AttendanceModel.permission;

    vm.currentPage = 1;
    vm.pageSize = 20;

    vm.filter = AttendanceModel.filter;

    if (vm.filter.employeeId) {
        CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
            vm.filter.employee = CommonModel.employee;
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

    CommonModel.getDepartment().then(function () {
        vm.departments = CommonModel.departments;
        vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
    });

    AttendanceModel.getViolationType().then(function () {
        vm.violationOptions = AttendanceModel.violations;
    });

    AttendanceModel.getUserPermissionOnAttendanceSummary().then(function () {
        vm.permission = AttendanceModel.permission;
    });

    var getAttendanceSummary = function (startDate, endDate, departmentId, isLocal, employeeId, reasonId) {
        AttendanceModel.getAttendanceSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId).then(function () {
            vm.attendanceEmployees = AttendanceModel.attendanceEmployees;
            utilitiesFactory.searchOnPage(vm, vm.attendanceEmployees);
        });
    };

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

    getAttendanceSummary(vm.filter.startDate, vm.filter.endDate, vm.filter.departmentId, vm.filter.isLocal, vm.filter.employeeId, vm.filter.reasonId);

    vm.query = function (filter) {
        var employeeId = null;
        vm.searchKey = '';
        vm.currentPage = 1;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
            vm.filter.employeeId = employeeId;
        }

        AttendanceModel.cacheFilter(vm.filter);
        getAttendanceSummary(filter.startDate, filter.endDate, filter.departmentId, filter.isLocal, employeeId, vm.filter.reasonId);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.addOtherViolations = function (employee) {
        AttendanceModel.openPopupOtherViolation(vm, employee, vm.filter.startDate, vm.filter.endDate).then(function () { }, function () {
            getAttendanceSummary(vm.filter.startDate, vm.filter.endDate, vm.filter.departmentId, vm.filter.isLocal, vm.filter.employeeId, vm.filter.reasonId);
        })['finally'](function () {
            vm.modalInstance = '';
        });
    };

    vm.adjust = function (employee) {
        var startDate = vm.filter.startDate,
            endDate = vm.filter.endDate;
        vm.attendanceDetailUrl = baseUrl + 'Discipline/Detail/' + employee.Employee.EmployeeId
            + '?startDate=' + startDate.toDateString() + '&endDate=' + endDate.toDateString() + '&reasonId=' + vm.filter.reasonId + '&isLocal=' + vm.filter.isLocal;
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
}]);

attendanceModule.controller("OtherViolationsController",
['$scope', '$modalInstance', 'AttendanceModel', 'employee', 'startDate', 'endDate', 'utilitiesFactory', 'CommonModel',
function ($scope, $modalInstance, AttendanceModel, employee, startDate, endDate, utilitiesFactory, CommonModel) {
    var vm = this,
        date = new Date();

    vm.employee = employee;
    vm.startDate = startDate;
    vm.endDate = endDate;

    vm.otherViolations = AttendanceModel.otherViolations;
    vm.otherViolationTypes = AttendanceModel.otherViolationTypes;

    vm.violation = {
        EmployeeId: vm.employee ? vm.employee.Employee.EmployeeId : null,
        Name: null,
        Score: 0,
        WorkDate: date,
        Comment: null,
        employee: null
    };

    vm.employeeIds = vm.violation.EmployeeId;

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

    AttendanceModel.getOtherViolationType(true).then(function () {
        vm.otherViolationTypes = AttendanceModel.otherViolationTypes;
    });

    vm.getEmployee = function () {
        return CommonModel.getAllEmployee(vm.searchEmployee, null, null, null, true, true);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.violation.employee) {
                vm.violation.employee = null;
                vm.violation.EmployeeId = null;

                if (vm.employeeIds) {
                    getOtherViolations(vm.employeeIds, vm.startDate, vm.endDate);
                }
                else {
                    vm.otherViolations = [];
                }
            }
        }
    };

    $scope.$watch(function () { return vm.violation.employee; },
        function (newVal) {
            if (newVal) {
                var employeeIdsTemp = "";
                employeeIdsTemp = vm.employeeIds ? vm.employeeIds + ',' + vm.violation.employee.EmployeeId : vm.violation.employee.EmployeeId;
                getOtherViolations(employeeIdsTemp, vm.startDate, vm.endDate);
            }
        });

    vm.setNameAndScore = function (otherViolationTypeId) {
        if (otherViolationTypeId) {
            var index = vm.otherViolationTypes.map(function (e) { return e.Id; }).indexOf(otherViolationTypeId),
                violation = vm.otherViolationTypes[index];

            vm.violation.Name = violation.Name;
            vm.violation.Score = violation.Value;
        }
        else {
            vm.violation.Name = null;
            vm.violation.Score = 0;
        }
    };

    var getOtherViolations = function (employeeIds, startDate, endDate) {
        AttendanceModel.getOtherViolations(employeeIds, startDate, endDate).then(function () {
            vm.otherViolations = AttendanceModel.otherViolations;
        });
    };

    if (vm.employee) {
        getOtherViolations(vm.employee.Employee.EmployeeId, vm.startDate, vm.endDate);
    }
    else {
        vm.otherViolations = [];
        vm.filteredItems = [];
    }

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.addViolation = function (violation) {
        if (violation.employee) {
            violation.EmployeeId = violation.employee.EmployeeId;
        }

        if (AttendanceModel.validateOtherViolation(violation)) {
            AttendanceModel.insertOtherViolation(violation).then(function () {
                vm.employeeIds += vm.employeeIds ? ',' + vm.violation.EmployeeId : vm.violation.EmployeeId;
                getOtherViolations(vm.employeeIds, vm.startDate, vm.endDate);
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
['$scope', '$window', '$routeParams', 'AttendanceModel', 'utilitiesFactory', 'CommonModel',
function ($scope, $window, $routeParams, AttendanceModel, utilitiesFactory, CommonModel) {
    var vm = this,
        date = new Date(),
        firstDate = new Date(date.getFullYear(), date.getMonth(), 1);

    vm.currentPage = 1;
    vm.pageSize = 15;

    vm.violationOptions = AttendanceModel.violations;
    vm.attendanceDetails = AttendanceModel.attendanceDetails;
    vm.permission = AttendanceModel.permission;

    vm.fromSummaryPage = $routeParams.id ? true : false;

    vm.filter = {
        startDate: $routeParams.startDate ? new Date($routeParams.startDate) : firstDate,
        endDate: $routeParams.endDate ? new Date($routeParams.endDate) : date,
        employee: null,
        departmentId: null,
        isLocal: $routeParams.isLocal ? $routeParams.isLocal : '',
        employeeId: $routeParams.id ? parseInt($routeParams.id) : null,
        reasonId: parseInt($routeParams.reasonId)
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
    utilitiesFactory.getTimeFormat(vm);

    CommonModel.getDepartment().then(function () {
        vm.departments = CommonModel.departments;
        vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
    });

    CommonModel.getEmployeeById(vm.filter.employeeId).then(function () {
        vm.filter.employee = CommonModel.employee;
        vm.filter.employee.Name = vm.filter.employee.NickName;
        vm.searchEmployee = vm.filter.employee.NickName + ' (' + vm.filter.employee.FullName + ')';
    });

    AttendanceModel.getViolationType().then(function () {
        vm.violationOptions = AttendanceModel.violations;
    });

    AttendanceModel.getUserPermissionOnAttendanceSummary().then(function () {
        vm.permission = AttendanceModel.permission;
    });

    var getAttendanceDetail = function (startDate, endDate, employeeId, departmentId, isLocal, reasonId) {
        AttendanceModel.getAttendanceDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId).then(function () {
            vm.attendanceDetails = AttendanceModel.attendanceDetails;
            vm.attendanceDetails = AttendanceModel.convertCheckInOutToDate(vm, vm.attendanceDetails);
            utilitiesFactory.searchOnPage(vm, vm.attendanceDetails);
        });
    };

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

    getAttendanceDetail(vm.filter.startDate, vm.filter.endDate, vm.filter.employeeId, vm.filter.departmentId, vm.filter.isLocal, vm.filter.reasonId);

    vm.query = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        getAttendanceDetail(vm.filter.startDate, vm.filter.endDate, employeeId, vm.filter.departmentId, vm.filter.isLocal, vm.filter.reasonId);
    };

    vm.calculateApproverScore = function (attendance, violation) {
        if (violation.HasViolated) {
            if (violation.HasIgnored) {
                attendance.ApproverSubtractScore -= violation.Score;
            }
            else {
                attendance.ApproverSubtractScore += violation.Score;
            }
        }
    };

    vm.save = function () {
        var violations = AttendanceModel.getIgnoredViolation(vm.attendanceDetails);
        AttendanceModel.openPopupUpdateIgnoredViolation(violations);
    };

    vm.backToList = function () {
        $window.history.back();
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
}]);