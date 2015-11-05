'use strict';
/*global periodModule*/

periodModule.factory('periodFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getPeriod: function (targetTypeId, isActive) {
            return $http.get(baseUrl + 'api/Period/GetPeriod', { params: { targetTypeId: targetTypeId, isActive: isActive } });
        },
        createPeriod: function (period) {
            return $http.post(baseUrl + 'api/Period/CreatePeriod', period);
        },
        updateBasicInformationPeriod: function (period) {
            return $http.put(baseUrl + 'api/Period/UpdateBasicInformationPeriod', period);
        },
        getPeriodById: function (periodId) {
            return $http.get(baseUrl + 'api/Period/GetPeriodById', { params: { periodId: periodId } });
        },
        updateStatusPeriod: function (period) {
            return $http.put(baseUrl + 'api/Period/UpdateStatusPeriod', period);
        },
        deletePeriod: function (periodId) {
            return $http['delete'](baseUrl + 'api/Period/DeletePeriod', { params: { periodId: periodId } });
        },
        getPeriodEmployeeSettingById: function (periodId, departmentId, isExcludedEmployee, isGeneratedEPA, isEvaluated) {
            return $http.get(baseUrl + 'api/Period/GetPeriodEmployeeSettingById',
                {
                    params:
                      {
                          periodId: periodId,
                          departmentId: departmentId,
                          isExcludedEmployee: isExcludedEmployee,
                          isGeneratedEPA: isGeneratedEPA,
                          isEvaluated: isEvaluated
                      }
                });
        },
        addEmployeeToPeriod: function (employeeInfo) {
            return $http.post(baseUrl + 'api/Period/AddEmployeToPeriod', employeeInfo);
        },
        updateEmployeeSettingOfPeriod: function (employeeSetting) {
            return $http.put(baseUrl + 'api/Period/UpdateEmployeeSettingOfPeriod', employeeSetting);
        },
        updateIncludeStatusEmployeePeriod: function (employeeStatusSetting) {
            return $http.put(baseUrl + 'api/Period/UpdateIncludeStatusEmployeePeriod', employeeStatusSetting);
        },
        getPermissionOnPeriod: function () {
            return $http.get(baseUrl + 'api/Period/GetUserPermissionOnPeriod');
        },
        generateEvaluation: function (period) {
            return $http.post(baseUrl + 'api/Period/GenerateEvaluation', period);
        },
        getProcessSettingByPeriodId: function (periodId) {
            return $http.get(baseUrl + 'api/Period/GetProcessSettingByPeriodId', { params: { periodId: periodId } });
        },
        updatePeriodProcessSetting: function (periodProcessSetting) {
            return $http.put(baseUrl + 'api/Period/UpdatePeriodProcessSetting', periodProcessSetting);
        },
        checkSelfProcessStarted: function (periodId, employeeIds) {
            return $http.get(baseUrl + 'api/Period/CheckSelfProcessStarted',
                {
                    params:
                      {
                          periodId: periodId,
                          employeeIds: employeeIds
                      }
                });
        }
    };
}]);