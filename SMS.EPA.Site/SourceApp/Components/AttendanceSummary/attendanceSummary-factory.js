'use strict';
/*global attendanceModule*/

attendanceModule.factory('attendanceFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getAttendanceSummary: function (startDate, endDate, departmentId, isLocal, employeeId, reasonId, showAll) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetAttendanceSummary',
                {
                    params:
                      {
                          startDate: startDate,
                          endDate: endDate,
                          showAll: showAll,
                          departmentId: departmentId,
                          isLocal: isLocal,
                          employeeId: employeeId,
                          reasonId: reasonId
                      }
                });
        },
        getOtherViolations: function (employeeIds, startDate, endDate, workDate) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetOtherViolations',
                {
                    params: {
                        employeeIds: employeeIds,
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
        getAttendanceDetail: function (startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored, showAll) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetAttendanceDetail',
                {
                    params: {
                        startDate: startDate,
                        endDate: endDate,
                        employeeId: employeeId,
                        departmentId: departmentId,
                        reasonId: reasonId,
                        isLocal: isLocal,
                        hasIgnored: hasIgnored,
                        showAll: showAll
                    }
                }
            );
        },
        ignoredViolation: function (violations) {
            return $http.put(baseUrl + 'api/AttendanceSummary/UpdateIgnoredViolation', violations);
        },
        getUserPermissionOnAttendanceSummary: function () {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetUserPermissionOnAttendanceSummary');
        },
        getOtherViolationType: function (isActive) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetOtherViolationType', { params: { isActive: isActive }});
        },
        checkUserPermissionIsAdmin: function () {
            return $http.get(baseUrl + 'api/AttendanceSummary/CheckUserPermissionIsAdmin');
        }
    };
 }]);