'use strict';
/*global commonModule*/

commonModule.factory('CommonModel', ['commonFactory', 'errorFactory',
function (commonFactory, errorFactory) {
    var commonModel = {};

    commonModel.targetTypes = [];
    commonModel.systemRoles = [];
    commonModel.kpis = [];
    commonModel.kpiGroups = [];
    commonModel.kpiGroup = {};
    commonModel.groups = [];
    commonModel.employee = {};
    commonModel.departments = [];
    commonModel.treeDepartment = {};

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
    commonModel.getKPI = function (targetTypeId, kpiGroupId, isActive, isRequiredEvaluate) {
        return commonFactory.getKPI(targetTypeId, kpiGroupId, isActive, isRequiredEvaluate)
        .success(function (kpis) {
            commonModel.kpis = kpis;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get KPI Group.
    commonModel.getKPIGroup = function (targetTypeId, isActive) {
        return commonFactory.getKPIGroup(targetTypeId, isActive)
        .success(function (kpiGroups) {
            commonModel.kpiGroups = kpiGroups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get KPI Group By Id.
    commonModel.getKPIGroupById = function (kpiGroupId) {
        return commonFactory.getKPIGroupById(kpiGroupId)
        .success(function (kpiGroup) {
            commonModel.kpiGroup = kpiGroup;
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

    /// Get employee by id.
    commonModel.getEmployeeById = function (employeeId) {
        return commonFactory.getEmployeeById(employeeId)
        .success(function (employee) {
            commonModel.employee = employee;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    var createLabel = function (item, level) {
        var label = '',
			space = '',
			levelTemp = level;

        if (level > 0) {
            while (levelTemp > 0) {
                label = label + " " + space;
                space = String.fromCharCode(160) + String.fromCharCode(160) + String.fromCharCode(160);
                levelTemp--;
            }

            label = label + item.Name;
        }
        else {
            label = item.Name;
        }

        return label;
    };

    var generateTreeDepartment = function (departments, options, level) {
        angular.forEach(departments, function (item) {
            options.push({ Name: createLabel(item, level), DepartmentId: item.DepartmentId });
            generateTreeDepartment(item.Childrens, options, level + 1);
        });

        return options;
    };

    /// Get department.
    commonModel.getDepartment = function (parentId, isActive) {
        return commonFactory.getDepartment(parentId, isActive)
        .success(function (data) {
            commonModel.treeDepartment = generateTreeDepartment(data.TreeDepartment.Childrens, [], 0);
            commonModel.departments = data.Departments;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return commonModel;
} ]);