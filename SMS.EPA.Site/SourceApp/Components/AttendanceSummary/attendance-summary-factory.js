(function (angular) {
    'use strict';

    angular.module('attendanceModule').factory('attendanceFactory', attendanceFactory);
    attendanceFactory.$inject = ['$http', 'baseUrl'];

    function attendanceFactory($http, baseUrl) {
        return {
            getAttendanceDetail: getAttendanceDetail,
            getAttendanceSummary: getAttendanceSummary,
            getOtherViolations: getOtherViolations,
            getUserPermissionOnAttendanceSummary: getUserPermissionOnAttendanceSummary,
            getViolationType: getViolationType
        };

        function getAttendanceDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored) {
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
        }

        function getAttendanceSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId) {
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
        }

        function getOtherViolations(employeeId, startDate, endDate, workDate) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetOtherViolations',
                {
                    params: {
                        employeeId: employeeId,
                        startDate: startDate,
                        endDate: endDate,
                        workDate: workDate
                    }
                });
        }

        function getUserPermissionOnAttendanceSummary() {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetUserPermissionOnAttendanceSummary');
        }

        function getViolationType(includeOtherViolate, isActive) {
            return $http.get(baseUrl + 'api/AttendanceSummary/GetViolationType',
                {
                    params: {
                        includeOtherViolate: includeOtherViolate,
                        isActive: isActive
                    }
                });
        }
    }
})(angular);