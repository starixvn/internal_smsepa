'use strict';
/*global attendanceModule*/

attendanceModule.factory('attendanceFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getAttendanceSummary: function (startDate, endDate, departmentId, employeeId, reasonId) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetAttendanceSummary',
                {
                    params:
                      {
                          startDate: startDate,
                          endDate: endDate,
                          departmentId: departmentId,
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
        insertOtherViolation: function (violation) {
            return $http.post(baseUrl + 'api/AttendanceSummary/InsertOtherViolation', violation);
        },
        deleteOtherViolation: function (employeeId, workDate, name) {
            return $http["delete"](baseUrl + 'api/AttendanceSummary/DeleteOtherViolation',
                {
                    params: {
                        employeeId: employeeId,
                        workDate: workDate,
                        name: name
                    }
                });
        },
        getViolationType: function () {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetViolationType');
        },
        getAttendanceDetail: function (startDate, endDate, employeeId, departmentId, reasonId) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetAttendanceDetail',
                {
                    params: { 
                        startDate: startDate,
                        endDate: endDate,
                        employeeId: employeeId,
                        departmentId: departmentId,
                        reasonId: reasonId
                    }
                }
            );
        },
        ignoredViolation: function (violations) {
            return $http.put(baseUrl + 'api/AttendanceSummary/UpdateIgnoredViolation', violations);
        }
    }
}]);