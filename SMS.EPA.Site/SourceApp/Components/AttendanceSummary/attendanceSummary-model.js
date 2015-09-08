'use strict';
/*global attendanceModule*/

attendanceModule.factory('AttendanceModel', ['attendanceFactory', 'errorFactory', function (attendanceFactory, errorFactory) {
    var attendanceModel = {};

    attendanceModel.attendanceEmployees = [];

    attendanceModel.getAttendanceSummary = function (startDate, endDate, departmentId, employeeId) {
        return attendanceFactory.getAttendanceSummary(startDate, endDate, departmentId, employeeId).success(function (attendanceEmployees) {
            attendanceModel.attendanceEmployees = attendanceEmployees;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return attendanceModel;
}]);