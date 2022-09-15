(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('commonModule').factory('commonFactory', commonFactory);
    commonFactory.$inject = ['$http', 'baseUrl'];

    function commonFactory($http, baseUrl) {
        return {
            searchEmployee: function (query, teamIds, isSpecialUser, isDisabledUser, getByApprover, includeVerifier) {
                return $http.get(baseUrl + 'api/Common/SearchEmployee', {
                    params: { query, teamIds, isSpecialUser, isDisabledUser, getByApprover, includeVerifier }
                });
            },
            getTargetType: function (isActive, excludeTargetTypeIds) {
                return $http.get(baseUrl + 'api/Common/GetTargetType', {
                    params: {
                        isActive: isActive,
                        excludeTargetTypeIds: excludeTargetTypeIds
                    }
                });
            },
            getSystemRoles: function (exceptNone) {
                return $http.get(baseUrl + 'api/Common/GetSystemRoles', { params: { exceptNone: exceptNone } });
            },
            getKPI: function (kpiGroupId, isActive, isRequiredEvaluate) {
                return $http.get(baseUrl + 'api/Common/GetKPI', {
                    params: {
                        kpiGroupId: kpiGroupId,
                        isActive: isActive,
                        isRequiredEvaluate: isRequiredEvaluate
                    }
                });
            },
            getKPIGroup: function (isActive) {
                return $http.get(baseUrl + 'api/Common/GetKPIGroup', { params: { isActive: isActive } });
            },
            getEmployeeById: function (employeeId) {
                return $http.get(baseUrl + 'api/Common/GetEmployeeById', { params: { employeeId: employeeId } });
            },
            getAllGroup: function () {
                return $http.get(baseUrl + 'api/Common/GetAllGroup');
            },
            getDepartment: function (parentId, isActive) {
                return $http.get(baseUrl + 'api/Common/GetDepartment', { params: { parentId: parentId, isActive: isActive } });
            },
            getAllStatus: function () {
                return $http.get(baseUrl + 'api/Common/GetAllStatus');
            },
            getAllEmployee: function (query, groupId, isSpecialUser, isDisabledUser, getByApprover, includeVerifier) {
                return $http.get(baseUrl + 'api/Common/GetAllEmployee', {
                    params: {
                        query: query,
                        groupId: groupId,
                        isSpecialUser: isSpecialUser,
                        isDisabledUser: isDisabledUser,
                        getByApprover: getByApprover,
                        includeVerifier: includeVerifier
                    }
                });
            },
            getEmployeeByDepartmentId: function (departmentId) {
                return $http.get(baseUrl + 'api/Common/GetEmployeeByDepartmentId', { params: { departmentId: departmentId } });
            },
            getEmployeeByGroupId: function (groupId) {
                return $http.get(baseUrl + 'api/Common/GetEmployeeByGroupId', { params: { groupId: groupId } });
            },
            getPeriod: function (targetTypeId, isActive) {
                return $http.get(baseUrl + 'api/Common/GetPeriod', {
                    params: {
                        targetTypeId: targetTypeId,
                        isActive: isActive
                    }
                });
            },
            getStandardGrade: function () {
                return $http.get(baseUrl + 'api/Common/GetStandardGrade');
            },
            getAllPeriodIncludeImport: function () {
                return $http.get(baseUrl + 'api/Common/GetAllPeriodIncludeImport');
            },
            getTargetTemplate: function (targetTypeId, isActive) {
                return $http.get(baseUrl + 'api/Common/GetAllTargetTemplate', {
                    params: {
                        targetTypeId: targetTypeId,
                        isActive: isActive
                    }
                });
            },
            getAllMonthlyPeriod: function () {
                return $http.get(baseUrl + 'api/MonthlyPeriod/GetAllMonthlyPeriod');
            },
            getMenusByEmployeeId: function () {
                return $http.get(baseUrl + 'api/Common/GetMenusByEmployeeId');
            },
            getAllJobTitle: function () {
                return $http.get(baseUrl + 'api/Common/GetAllJobTitle');
            },
            getUserPermissionOnFlowManagement: function () {
                return $http.get(baseUrl + 'api/Common/GetUserPermissionOnFlowManagement');
            },
            getJobLevel: function (isDeleted) {
                return $http.get(baseUrl + 'api/Common/GetJobLevel', { params: { isDeleted: isDeleted } });
            },
            getOrgChart: function () {
                return $http.get(baseUrl + 'api/Common/GetOrgChart');
            }
        };
    }
})(angular);