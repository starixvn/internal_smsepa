'use strict';
/*global attendanceModule*/

attendanceModule.factory('AttendanceModel',
['$modal', 'attendanceFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory',
function ($modal, attendanceFactory, errorFactory, notificationFactory, utilitiesFactory) {
    var attendanceModel = {};

    attendanceModel.attendanceEmployees = [];
    attendanceModel.otherViolations = [];

    attendanceModel.getAttendanceSummary = function (startDate, endDate, departmentId, employeeId) {
        return attendanceFactory.getAttendanceSummary(startDate, endDate, departmentId, employeeId).success(function (attendanceEmployees) {
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
        return attendanceFactory.getOtherViolations(employeeId, startDate, endDate, workDate).success(function (violations) {
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
    }

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
    }

    attendanceModel.validateOtherViolation = function (violation) {
        if (!violation) {
            return false;
        }
        else {
            if (!violation.Name) {
                notificationFactory.error("NameRequried");
                return false;
            }
            else if (!violation.WorkDate) {
                notificationFactory.error("WorkDateRequried");
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
            return attendanceFactory.deleteOtherViolation(vm.employee.EmployeeId, violation.WorkDate, violation.Name).success(function (response) {
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

    return attendanceModel;
}]);