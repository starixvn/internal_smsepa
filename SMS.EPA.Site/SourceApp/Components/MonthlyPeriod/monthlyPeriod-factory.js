'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.factory('monthlyPeriodFactory', ['$http', 'baseUrl', function ($http, baseUrl) {
    return {
        getMonthlyPeriod: function (fromDate, toDate) {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetMonthlyPeriod',
                {
                    params: {
                        fromDate: fromDate,
                        toDate: toDate
                    }
                });
        },
        getMonthlyPeriodById: function (periodId) {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetMonthlyPeriodById', { params: { periodId: periodId } });
        },
        createMonthlyPeriod: function (period) {
            return $http.post(baseUrl + 'api/MonthlyPeriod/CreateMonthlyPeriod', period);
        },
        updateBasicInfoMonthlyPeriod: function (period) {
            return $http.put(baseUrl + 'api/MonthlyPeriod/UpdateBasicInfoMonthlyPeriod', period);
        },
        getProcessSettingByPeriodId: function (periodId) {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetProcessSettingByPeriodId', { params: { periodId: periodId } });
        },
        updateProcessSettingMonthlyPeriod: function (processSetting) {
            return $http.put(baseUrl + 'api/MonthlyPeriod/UpdateProcessSettingMonthlyPeriod', processSetting);
        },
        deleteMonthlyPeriod: function (periodId) {
            return $http['delete'](baseUrl + 'api/MonthlyPeriod/DeleteMonthlyPeriod', { params: { periodId: periodId } });
        },
        getEmployeeSettingMonthlyPeriodById: function (periodId, departmentId, isLocal, isExcludedEmployee, isGeneratedEPA, isEvaluated) {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetEmployeeSettingMonthlyPeriodById',
                {
                    params:
                      {
                          periodId: periodId,
                          departmentId: departmentId,
                          isLocal: isLocal,
                          isExcludedEmployee: isExcludedEmployee,
                          isGeneratedEPA: isGeneratedEPA,
                          isEvaluated: isEvaluated
                      }
                });
        },
        addEmployeeToMonthlyPeriod: function (employeeInfo) {
            return $http.post(baseUrl + 'api/MonthlyPeriod/AddEmployeeToMonthlyPeriod', employeeInfo);
        },
        updateEmployeeSettingOfPeriod: function (employeeSetting) {
            return $http.put(baseUrl + 'api/MonthlyPeriod/UpdateEmployeeSettingOfPeriod', employeeSetting);
        },
        generateEvaluation: function (period) {
            return $http.post(baseUrl + 'api/MonthlyPeriod/GenerateEvaluation', period);
        },
        updateIncludeStatusEmployeePeriod: function (employeeStatusSetting) {
            return $http.put(baseUrl + 'api/MonthlyPeriod/UpdateIncludeStatusEmployeePeriod', employeeStatusSetting);
        },
        checkSelfProcessStarted: function (periodId, employeeIds) {
            return $http.get(baseUrl + 'api/MonthlyPeriod/CheckSelfProcessStarted',
            {
                params:
                  {
                      periodId: periodId,
                      employeeIds: employeeIds
                  }
            });
        },
        getPermissionOnMonthlyPeriod: function () {
            return $http.get(baseUrl + 'api/MonthlyPeriod/GetUserPermissionOnMonthlyPeriod');
        }
    };
}]);