'use strict';
/*global attendanceModule*/

attendanceModule.factory('attendanceFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getAttendanceSummary: function (startDate, endDate, departmentId, employeeId) {
            return $http.get(baseUrl + 'api/AttendanceSummary',
                {
                    params:
                      {
                          startDate: startDate,
                          endDate: endDate,
                          departmentId: departmentId,
                          employeeId: employeeId
                      }
                });
        }
    }
}]);