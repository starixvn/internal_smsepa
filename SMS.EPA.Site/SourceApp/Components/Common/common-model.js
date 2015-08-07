'use strict';
/*global commonModule*/

commonModule.factory('CommonModel', ['commonFactory', 'errorFactory',
function (commonFactory, errorFactory) {
    var commonModel = {};

    commonModel.targetTypes = [];
    commonModel.systemRoles = [];
    commonModel.kpis = [];
    commonModel.kpiGroups = [];
    commonModel.groups = [];
    commonModel.employees = [];
    commonModel.employee = {};
    commonModel.departments = [];
    commonModel.treeDepartment = {};
    commonModel.statusList = [];

    /// Get target type.
    commonModel.getTargetType = function (isActive) {
        return commonFactory.getTargetType(isActive)
        .success(function (targetTypes) {
            commonModel.targetTypes = targetTypes;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get index of row in table.
    commonModel.getIndex = function (index, currentPage, pageSize) {
        var indexOfRow = pageSize * (currentPage - 1) + index + 1;
        return indexOfRow;
    };

    commonModel.getSystemRoles = function (exceptNone) {
        return commonFactory.getSystemRoles(exceptNone).success(function (systemRoles) {
            commonModel.systemRoles = systemRoles;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get KPI.
    commonModel.getKPI = function (kpiGroupId, isActive, isRequiredEvaluate) {
        return commonFactory.getKPI(kpiGroupId, isActive, isRequiredEvaluate)
        .success(function (kpis) {
            commonModel.kpis = kpis;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get KPI Group.
    commonModel.getKPIGroup = function (isActive) {
        return commonFactory.getKPIGroup(isActive)
        .success(function (kpiGroups) {
            commonModel.kpiGroups = kpiGroups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get group.
    commonModel.getAllGroup = function () {
        return commonFactory.getAllGroup().success(function (groups) {
            commonModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get all employee.
    commonModel.getAllEmployee = function (query, groupId, isSpecialUser, isDisabledUser) {
        return commonFactory.getAllEmployee(query, groupId, isSpecialUser, isDisabledUser)
        .success(function (employees) {
            commonModel.employees = employees;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get employee by department id.
    commonModel.getEmployeeByDepartmentId = function (departmentId) {
        return commonFactory.getEmployeeByDepartmentId(departmentId)
        .success(function (employees) {
            commonModel.employees = employees;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get employee by group id.
    commonModel.getEmployeeByGroupId = function (groupId) {
        return commonFactory.getEmployeeByGroupId(groupId)
        .success(function (employees) {
            commonModel.employees = employees;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get employee by id.
    commonModel.getEmployeeById = function (employeeId) {
        return commonFactory.getEmployeeById(employeeId)
        .success(function (employee) {
            commonModel.employee = employee;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get department.
    commonModel.getDepartment = function (parentId, isActive) {
        return commonFactory.getDepartment(parentId, isActive)
        .success(function (data) {
            commonModel.treeDepartment = data.TreeDepartment;
            commonModel.departments = data.Departments;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get all status.
    commonModel.getAllStatus = function () {
        return commonFactory.getAllStatus()
        .success(function (statusList) {
            commonModel.statusList = statusList;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return commonModel;
} ]);