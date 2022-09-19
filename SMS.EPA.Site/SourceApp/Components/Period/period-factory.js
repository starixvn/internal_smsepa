(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('periodModule').factory('periodFactory', periodFactory);
    periodFactory.$inject = ['$http', 'baseUrl'];

    function periodFactory($http, baseUrl) {
        return {
            checkSelfProcessStarted,
            createPeriod,
            deletePeriod,
            generateEvaluation,
            getPeriod,
            getPeriodById,
            getPeriodEmployeeSettingById,
            getPermissionOnPeriod,
            getProcessSettingByPeriodId,
            updateBasicInformationPeriod,
            addEmployeeSetting,
            updateEmployeeSettingOfPeriod,
            updateIncludeStatusEmployeePeriod,
            updatePeriodProcessSetting,
            updateStatusPeriod
        };

        function addEmployeeSetting(params) {
            return $http.post(baseUrl + 'api/Period/AddEmployeToPeriod', params);
        }

        function checkSelfProcessStarted(periodId, employeeIds) {
            return $http.get(baseUrl + 'api/Period/CheckSelfProcessStarted', {
                params: { periodId, employeeIds }
            });
        }

        function createPeriod(period) {
            return $http.post(baseUrl + 'api/Period/CreatePeriod', period);
        }

        function deletePeriod(periodId) {
            return $http['delete'](baseUrl + 'api/Period/DeletePeriod', { params: { periodId: periodId } });
        }

        function generateEvaluation(period) {
            return $http.post(baseUrl + 'api/Period/GenerateEvaluation', period);
        }

        function getPeriod(targetTypeId, isActive) {
            return $http.get(baseUrl + 'api/Period/GetPeriod', { params: { targetTypeId: targetTypeId, isActive: isActive } });
        }

        function getPeriodById(periodId) {
            return $http.get(baseUrl + 'api/Period/GetPeriodById', { params: { periodId: periodId } });
        }

        function getPeriodEmployeeSettingById(periodId, teamIds, isLocal, isExcludedEmployee, isGeneratedEPA, isEvaluated, hasMonthlyEPA, epaStatus) {
            return $http.get(baseUrl + 'api/Period/GetPeriodEmployeeSettingById', {
                params: { periodId, teamIds, isLocal, isExcludedEmployee, isGeneratedEPA, isEvaluated, hasMonthlyEPA, epaStatus }
            });
        }

        function getPermissionOnPeriod() {
            return $http.get(baseUrl + 'api/Period/GetUserPermissionOnPeriod');
        }

        function getProcessSettingByPeriodId(periodId) {
            return $http.get(baseUrl + 'api/Period/GetProcessSettingByPeriodId', { params: { periodId: periodId } });
        }

        function updateBasicInformationPeriod(period) {
            return $http.put(baseUrl + 'api/Period/UpdateBasicInformationPeriod', period);
        }

        function updateEmployeeSettingOfPeriod(employeeSetting) {
            return $http.put(baseUrl + 'api/Period/UpdateEmployeeSettingOfPeriod', employeeSetting);
        }

        function updateIncludeStatusEmployeePeriod(employeeStatusSetting) {
            return $http.put(baseUrl + 'api/Period/UpdateIncludeStatusEmployeePeriod', employeeStatusSetting);
        }

        function updatePeriodProcessSetting(periodProcessSetting) {
            return $http.put(baseUrl + 'api/Period/UpdatePeriodProcessSetting', periodProcessSetting);
        }

        function updateStatusPeriod(period) {
            return $http.put(baseUrl + 'api/Period/UpdateStatusPeriod', period);
        }
    }
})(angular);