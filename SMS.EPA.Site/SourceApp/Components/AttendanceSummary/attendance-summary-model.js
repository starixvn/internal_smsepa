(function (angular) {
    'use strict';

    angular.module('attendanceModule').factory('AttendanceModel', attendanceModel);
    attendanceModel.$inject = ['$modal', 'attendanceFactory', 'errorFactory'];

    function attendanceModel($modal, attendanceFactory, errorFactory) {
        var model = {},
            date = new Date();

        model.rangeDateFilter = {
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

        model.attendanceEmployees = [];
        model.otherViolations = [];
        model.violationTypes = [];
        model.attendanceDetails = [];
        model.permission = null;
        model.summaryPaging = {
            currentPage: 1,
            pageSize: 20
        };

        model.setSummaryPaging = function (currentPage, pageSize) {
            model.summaryPaging.currentPage = currentPage;
            model.summaryPaging.pageSize = pageSize;
        };

        model.resetSummaryPaging = function () {
            model.summaryPaging.currentPage = 1;
            model.summaryPaging.pageSize = 20;
        };

        model.getRangeDateFilter = function () {
            var startDate = new Date(),
                endDate = new Date(),
                date = new Date(),
                dateNoTime = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0);

            if (dateNoTime >= model.rangeDateFilter.firstRangeStartDate && dateNoTime <= model.rangeDateFilter.firstRangeEndDate) {
                startDate = model.rangeDateFilter.firstStartDate;
                endDate = model.rangeDateFilter.firstEndDate;
            }
            else if (dateNoTime >= model.rangeDateFilter.secondRangeStartDate && dateNoTime <= model.rangeDateFilter.secondRangeEndDate) {
                startDate = model.rangeDateFilter.secondStartDate;
                endDate = model.rangeDateFilter.secondEndDate;
            }
            else if (dateNoTime >= model.rangeDateFilter.thirdRangeStartDate && dateNoTime <= model.rangeDateFilter.thirdRangeEndDate) {
                startDate = model.rangeDateFilter.thirdStartDate;
                endDate = model.rangeDateFilter.thirdEndDate;
            }

            return {
                startDate: startDate,
                endDate: endDate
            };
        };

        model.getAttendanceSummary = function (startDate, endDate, departmentId, isLocal, employeeId, reasonId) {
            return attendanceFactory.getAttendanceSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId)
            .success(function (attendanceEmployees) {
                model.attendanceEmployees = attendanceEmployees;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupOtherViolation = function (vm, employee, startDate, endDate) {
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

        model.getOtherViolations = function (employeeId, startDate, endDate, workDate) {
            return attendanceFactory.getOtherViolations(employeeId, startDate, endDate, workDate).success(function (violations) {
                model.otherViolations = violations;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getViolationType = function (includeOtherViolate, isActive) {
            return attendanceFactory.getViolationType(includeOtherViolate, isActive).success(function (violationTypes) {
                model.violationTypes = violationTypes;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAttendanceDetail = function (startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored) {
            return attendanceFactory.getAttendanceDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored)
            .success(function (attendanceDetails) {
                model.attendanceDetails = attendanceDetails;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getUserPermissionOnAttendanceSummary = function () {
            return attendanceFactory.getUserPermissionOnAttendanceSummary().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getDepartmentName = function (departmentId, departments) {
            var departmentName = '';

            angular.forEach(departments, function (department) {
                if (department.DepartmentId === departmentId) {
                    departmentName = department.Name;
                }
            });

            return departmentName;
        };

        return model;
    }
})(angular);