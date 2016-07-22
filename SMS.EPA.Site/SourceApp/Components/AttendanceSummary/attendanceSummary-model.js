'use strict';
/*global attendanceModule*/

attendanceModule.factory('AttendanceModel',
['$modal', 'attendanceFactory', 'errorFactory',
function ($modal, attendanceFactory, errorFactory) {
    var attendanceModel = {},
        date = new Date();

    attendanceModel.rangeDateFilter = {
        firstRangeStartDate: new Date(date.getFullYear(), 1, 1), // 1-2
        firstRangeEndDate: new Date(date.getFullYear(), 7, 0), // 31-07
        firstStartDate: new Date(date.getFullYear(), 0, 1), // 1-1
        firstEndDate: new Date(date.getFullYear(), 6, 0), // 30-06
        secondRangeStartDate: new Date(date.getFullYear(), 7, 1), // 1-8
        secondRangeEndDate: new Date(date.getFullYear() + 1, 1, 0), // 31-01-next year
        secondStartDate: new Date(date.getFullYear(), 6, 1), // 1-7
        secondEndDate: new Date(date.getFullYear(), 12, 0), // 31-12
        thirdRangeStartDate: new Date(date.getFullYear() - 1, 7, 1), // 1-8-last year
        thirdRangeEndDate: new Date(date.getFullYear(), 1, 0), // 31-1
        thirdStartDate: new Date(date.getFullYear() - 1, 6, 1), // 1-7-last year
        thirdEndDate: new Date(date.getFullYear() - 1, 12, 0) // 31-12-last year
    };

    attendanceModel.attendanceEmployees = [];
    attendanceModel.otherViolations = [];
    attendanceModel.violationTypes = [];
    attendanceModel.attendanceDetails = [];
    attendanceModel.permission = null;
    attendanceModel.summaryPaging = {
        currentPage: 1,
        pageSize: 20
    };

    attendanceModel.setSummaryPaging = function (currentPage, pageSize) {
        attendanceModel.summaryPaging.currentPage = currentPage;
        attendanceModel.summaryPaging.pageSize = pageSize;
    };

    attendanceModel.resetSummaryPaging = function () {
        attendanceModel.summaryPaging.currentPage = 1;
        attendanceModel.summaryPaging.pageSize = 20;
    };

    attendanceModel.getRangeDateFilter = function () {
        var startDate = new Date(),
            endDate = new Date(),
            date = new Date(),
            dateNoTime = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0);

        if (dateNoTime >= attendanceModel.rangeDateFilter.firstRangeStartDate && dateNoTime <= attendanceModel.rangeDateFilter.firstRangeEndDate) {
            startDate = attendanceModel.rangeDateFilter.firstStartDate;
            endDate = attendanceModel.rangeDateFilter.firstEndDate;
        }
        else if (dateNoTime >= attendanceModel.rangeDateFilter.secondRangeStartDate && dateNoTime <= attendanceModel.rangeDateFilter.secondRangeEndDate) {
            startDate = attendanceModel.rangeDateFilter.secondStartDate;
            endDate = attendanceModel.rangeDateFilter.secondEndDate;
        }
        else if (dateNoTime >= attendanceModel.rangeDateFilter.thirdRangeStartDate && dateNoTime <= attendanceModel.rangeDateFilter.thirdRangeEndDate) {
            startDate = attendanceModel.rangeDateFilter.thirdStartDate;
            endDate = attendanceModel.rangeDateFilter.thirdEndDate;
        }

        return {
            startDate: startDate,
            endDate: endDate
        };
    };

    attendanceModel.getAttendanceSummary = function (startDate, endDate, departmentId, isLocal, employeeId, reasonId) {
        return attendanceFactory.getAttendanceSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId)
        .success(function (attendanceEmployees) {
            attendanceModel.attendanceEmployees = attendanceEmployees;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    attendanceModel.openPopupOtherViolation = function (vm, employee, startDate, endDate) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/AttendanceSummary/Templates/otherViolation.html',
            controller: "OtherViolationsController as violationCtrl",
            resolve: {
                employee: function () {
                    return employee;
                },
                startDate: function () {
                    return startDate;
                },
                endDate: function () {
                    return endDate;
                }
            },
            backdrop: 'static',
            windowClass: "modal-confirm-center period-create-popup"
        });

        return vm.modalInstance.result;
    };

    attendanceModel.getOtherViolations = function (employeeId, startDate, endDate, workDate) {
        return attendanceFactory.getOtherViolations(employeeId, startDate, endDate, workDate)
        .success(function (violations) {
            attendanceModel.otherViolations = violations;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    attendanceModel.getViolationType = function (includeOtherViolate, isActive) {
        return attendanceFactory.getViolationType(includeOtherViolate, isActive)
        .success(function (violationTypes) {
            attendanceModel.violationTypes = violationTypes;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    attendanceModel.getAttendanceDetail = function (startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored) {
        return attendanceFactory.getAttendanceDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored)
        .success(function (attendanceDetails) {
            attendanceModel.attendanceDetails = attendanceDetails;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    attendanceModel.getUserPermissionOnAttendanceSummary = function () {
        return attendanceFactory.getUserPermissionOnAttendanceSummary().success(function (permission) {
            attendanceModel.permission = permission;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    attendanceModel.getDepartmentName = function (departmentId, departments) {
        var departmentName = '';

        angular.forEach(departments, function (department) {
            if (department.DepartmentId === departmentId) {
                departmentName = department.Name;
            }
        });

        return departmentName;
    };

    return attendanceModel;
}]);

attendanceModule.factory('AttendancePagingFactory', function () {
    var attendancePaging = {};

    attendancePaging.summaryPaging = {
        currentPage: 1,
        pageSize: 20
    };

    attendancePaging.setSummaryPaging = function (currentPage, pageSize) {
        attendancePaging.summaryPaging.currentPage = currentPage;
        attendancePaging.summaryPaging.pageSize = pageSize;
    };

    attendancePaging.resetSummaryPaging = function () {
        attendancePaging.summaryPaging.currentPage = 1;
        attendancePaging.summaryPaging.pageSize = 20;
    };

    return attendancePaging;
});