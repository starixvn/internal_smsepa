'use strict';
/*global attendanceModule*/

attendanceModule.factory('attendanceFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getAttendanceSummary: function (startDate, endDate, departmentId, employeeId) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetAttendanceSummary',
                {
                    params:
                      {
                          startDate: startDate,
                          endDate: endDate,
                          departmentId: departmentId,
                          employeeId: employeeId
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
        }
    }
}]);