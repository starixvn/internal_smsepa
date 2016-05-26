'use strict';
/*global attendanceModule*/

attendanceModule.factory('attendanceFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getAttendanceSummary: function (startDate, endDate, departmentId, isLocal, employeeId, reasonId) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetAttendanceSummary',
                {
                    params:
                      {
                          startDate: startDate,
                          endDate: endDate,
                          departmentId: departmentId,
                          isLocal: isLocal,
                          employeeId: employeeId,
                          reasonId: reasonId
                      }
                });
        },
        getOtherViolations: function (employeeId, startDate, endDate, workDate) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetOtherViolations',
                {
                    params: {
                        employeeId: employeeId,
                        startDate: startDate,
                        endDate: endDate,
                        workDate: workDate
                    }
                });
        },
        getViolationType: function (includeOtherViolate, isActive) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetViolationType',
                {
                    params: {
                        includeOtherViolate: includeOtherViolate,
                        isActive: isActive
                    }
                });
        },
        getAttendanceDetail: function (startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetAttendanceDetail',
                {
                    params: {
                        startDate: startDate,
                        endDate: endDate,
                        employeeId: employeeId,
                        departmentId: departmentId,
                        reasonId: reasonId,
                        isLocal: isLocal,
                        hasIgnored: hasIgnored
                    }
                });
        },
        getUserPermissionOnAttendanceSummary: function () {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetUserPermissionOnAttendanceSummary');
        }
    };
}]);