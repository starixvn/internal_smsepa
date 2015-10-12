'use strict';
/*global attendanceModule*/

attendanceModule.factory('AttendanceModel',
['$modal', 'attendanceFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory',
function ($modal, attendanceFactory, errorFactory, notificationFactory, utilitiesFactory) {
    var attendanceModel = {},
        date = new Date(),
        firstDate = new Date(date.getFullYear(), date.getMonth(), 1);

    attendanceModel.attendanceEmployees = [];
    attendanceModel.otherViolations = [];
    attendanceModel.violations = [];
    attendanceModel.attendanceDetails = [];
    attendanceModel.permission = null;

    attendanceModel.filter = {
        startDate: firstDate,
        endDate: date,
        employee: null,
        departmentId: null,
        employeeId: null,
        reasonId: null
    };

    attendanceModel.cacheFilter = function (filter) {
        attendanceModel.filter.startDate = filter.startDate;
        attendanceModel.filter.endDate = filter.endDate;
        attendanceModel.filter.departmentId = filter.departmentId;
        attendanceModel.filter.employeeId = filter.employeeId;
        attendanceModel.filter.reasonId = filter.reasonId;
    };

    attendanceModel.getAttendanceSummary = function (startDate, endDate, departmentId, employeeId, reasonId) {
        return attendanceFactory.getAttendanceSummary(startDate, endDate, departmentId, employeeId, reasonId).success(function (attendanceEmployees) {
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

    attendanceModel.getOtherViolations = function (employeeIds, startDate, endDate, workDate) {
        return attendanceFactory.getOtherViolations(employeeIds, startDate, endDate, workDate).success(function (violations) {
            attendanceModel.otherViolations = violations;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    attendanceModel.insertOtherViolation = function (violation) {
        return attendanceFactory.insertOtherViolation(violation).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    attendanceModel.deleteOtherViolation = function (violation) {
        return attendanceFactory.deleteOtherViolation(violation).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("DeletedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    attendanceModel.validateOtherViolation = function (violation) {
        if (!violation) {
            return false;
        }
        else {
            if (!violation.EmployeeId) {
                notificationFactory.error("EmployeeRequired");
                return false;
            }
            else if (!violation.Name) {
                notificationFactory.error("ViolationTypeRequired");
                return false;
            }
            else if (!violation.WorkDate) {
                notificationFactory.error("ViolationDateRequired");
                return false;
            }
        }

        return true;
    };

    attendanceModel.openPopupDeletePeriod = function (vm, violation) {
        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return violation.Name;
                },
                action: function () {
                    return 'delete';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            return attendanceFactory.deleteOtherViolation(violation.EmployeeId, violation.WorkDate, violation.Name).success(function (response) {
                if (response.IsSuccessful) {
                    var index = vm.otherViolations.indexOf(violation);
                    vm.otherViolations.splice(index, 1);
                    utilitiesFactory.searchOnPage(vm, vm.otherViolations);
                    notificationFactory.success("DeletedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    attendanceModel.getViolationType = function () {
        return attendanceFactory.getViolationType().success(function (violations) {
            attendanceModel.violations = violations;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    attendanceModel.getAttendanceDetail = function (startDate, endDate, employeeId, departmentId, reasonId) {
        return attendanceFactory.getAttendanceDetail(startDate, endDate, employeeId, departmentId, reasonId).success(function (attendanceDetails) {
            attendanceModel.attendanceDetails = attendanceDetails;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    var getIgnoredViolationItem = function (attendanceDetail) {
        var violations = [];

        if (attendanceDetail.CheckIn && attendanceDetail.CheckIn.HasViolated) {
            var violationCheckIn = {
                WorkDate: attendanceDetail.WorkDate,
                EmployeeId: attendanceDetail.Employee.EmployeeId,
                ViolationId: attendanceDetail.CheckIn.ViolationId,
                Ignored: attendanceDetail.CheckIn.HasIgnored
            };
            violations.push(violationCheckIn);
        }

        if (attendanceDetail.CheckOut && attendanceDetail.CheckOut.HasViolated) {
            var violationCheckOut = {
                WorkDate: attendanceDetail.WorkDate,
                EmployeeId: attendanceDetail.Employee.EmployeeId,
                ViolationId: attendanceDetail.CheckOut.ViolationId,
                Ignored: attendanceDetail.CheckOut.HasIgnored
            };
            violations.push(violationCheckOut);
        }

        if (attendanceDetail.LateIn && attendanceDetail.LateIn.HasViolated) {
            var violationLateIn = {
                WorkDate: attendanceDetail.WorkDate,
                EmployeeId: attendanceDetail.Employee.EmployeeId,
                ViolationId: attendanceDetail.LateIn.ViolationId,
                Ignored: attendanceDetail.LateIn.HasIgnored
            };
            violations.push(violationLateIn);
        }

        if (attendanceDetail.EarlyOut && attendanceDetail.EarlyOut.HasViolated) {
            var violationEarlyOut = {
                WorkDate: attendanceDetail.WorkDate,
                EmployeeId: attendanceDetail.Employee.EmployeeId,
                ViolationId: attendanceDetail.EarlyOut.ViolationId,
                Ignored: attendanceDetail.EarlyOut.HasIgnored
            };
            violations.push(violationEarlyOut);
        }

        if (attendanceDetail.TotalHours && attendanceDetail.TotalHours.HasViolated) {
            var violationTotalHours = {
                WorkDate: attendanceDetail.WorkDate,
                EmployeeId: attendanceDetail.Employee.EmployeeId,
                ViolationId: attendanceDetail.TotalHours.ViolationId,
                Ignored: attendanceDetail.TotalHours.HasIgnored
            };
            violations.push(violationTotalHours);
        }

        if (attendanceDetail.Absent && attendanceDetail.Absent.HasViolated) {
            var violationAbsent = {
                WorkDate: attendanceDetail.WorkDate,
                EmployeeId: attendanceDetail.Employee.EmployeeId,
                ViolationId: attendanceDetail.Absent.ViolationId,
                Ignored: attendanceDetail.Absent.HasIgnored
            };
            violations.push(violationAbsent);
        }

        if (attendanceDetail.UnpaidLeave && attendanceDetail.UnpaidLeave.HasViolated) {
            var violationUnpaidLeave = {
                WorkDate: attendanceDetail.WorkDate,
                EmployeeId: attendanceDetail.Employee.EmployeeId,
                ViolationId: attendanceDetail.UnpaidLeave.ViolationId,
                Ignored: attendanceDetail.UnpaidLeave.HasIgnored
            };
            violations.push(violationUnpaidLeave);
        }

        if (attendanceDetail.UnpaidLeaveAbsent && attendanceDetail.UnpaidLeaveAbsent.HasViolated) {
            var violationUnpaidLeaveAbsent = {
                WorkDate: attendanceDetail.WorkDate,
                EmployeeId: attendanceDetail.Employee.EmployeeId,
                ViolationId: attendanceDetail.UnpaidLeaveAbsent.ViolationId,
                Ignored: attendanceDetail.UnpaidLeaveAbsent.HasIgnored
            };
            violations.push(violationUnpaidLeaveAbsent);
        }

        if (attendanceDetail.ChangeShift && attendanceDetail.ChangeShift.HasViolated) {
            var violationChangeShift = {
                WorkDate: attendanceDetail.WorkDate,
                EmployeeId: attendanceDetail.Employee.EmployeeId,
                ViolationId: attendanceDetail.ChangeShift.ViolationId,
                Ignored: attendanceDetail.ChangeShift.HasIgnored
            };
            violations.push(violationChangeShift);
        }

        return violations;
    };

    attendanceModel.getIgnoredViolation = function (attendanceDetails) {
        var violations = [];

        angular.forEach(attendanceDetails, function (attendanceDetail) {
            var ignoredViolationItems = getIgnoredViolationItem(attendanceDetail);
            violations = violations.concat(ignoredViolationItems);
        });

        return violations;
    };

    attendanceModel.openPopupUpdateIgnoredViolation = function (violations) {
        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return "";
                },
                action: function () {
                    return 'save';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            return attendanceFactory.ignoredViolation(violations).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    var convertNumberToTime = function (number) {
        var sec_num = parseInt(number, 10), // don't forget the second param
            time = '';

        if (sec_num > 0) {
            var hours = Math.floor(sec_num / 60),
            minutes = Math.floor(sec_num - (hours * 60));

            if (hours < 10) {
                hours = "0" + hours;
            }

            if (minutes < 10) {
                minutes = "0" + minutes;
            }

            time = hours + ':' + minutes;
        }

        return time;
    };

    attendanceModel.convertCheckInOutToDate = function (attendanceDetails) {
        angular.forEach(attendanceDetails, function (attendanceDetail) {
            if (attendanceDetail.CheckIn != null && attendanceDetail.CheckIn.Value) {
                attendanceDetail.CheckIn.Value = new Date(attendanceDetail.CheckIn.Value);
            }

            if (attendanceDetail.CheckOut != null && attendanceDetail.CheckOut.Value) {
                attendanceDetail.CheckOut.Value = new Date(attendanceDetail.CheckOut.Value);
            }

            if (attendanceDetail.TotalHours != null && attendanceDetail.TotalHours.Value) {
                var totalHours = parseFloat(attendanceDetail.TotalHours.Value) * 60;
                attendanceDetail.TotalHours.Value = convertNumberToTime(totalHours);
            }

            if (attendanceDetail.LateIn != null && attendanceDetail.LateIn.Value) {
                attendanceDetail.LateIn.Value = convertNumberToTime(attendanceDetail.LateIn.Value);
            }

            if (attendanceDetail.EarlyOut != null && attendanceDetail.EarlyOut.Value) {
                attendanceDetail.EarlyOut.Value = convertNumberToTime(attendanceDetail.EarlyOut.Value);
            }
        });

        return attendanceDetails;
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