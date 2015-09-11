'use strict';
/*global attendanceModule*/

attendanceModule.factory('AttendanceModel',
['attendanceFactory', 'errorFactory', 'notificationFactory', '$modal',
function (attendanceFactory, errorFactory, notificationFactory, $modal) {
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

    attendanceModel.openPopupOtherViolation = function (employee, startDate, endDate) {
        var modalInstance = $modal.open({
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

        modalInstance.result.then(function () {
            modalInstance = null;
        });
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

    return attendanceModel;
}]);