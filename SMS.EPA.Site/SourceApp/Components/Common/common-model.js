(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('commonModule').factory('CommonModel', commonModel);
    commonModel.$inject = ['$uibModal', 'commonFactory', 'errorFactory'];

    function commonModel($uibModal, commonFactory, errorFactory) {
        var model = {};

        model.targetTypes = [];
        model.systemRoles = [];
        model.kpis = [];
        model.kpiGroups = [];
        model.groups = [];
        model.employees = [];
        model.employeesInDepartment = [];
        model.employeesInGroup = [];
        model.employee = {};
        model.departments = [];
        model.treeDepartment = {};
        model.statusList = [];
        model.periods = [];
        model.monthlyPeriods = [];
        model.standardGrades = [];
        model.periodOptions = [];
        model.otherViolationDetails = [];
        model.targetTemplates = [];
        model.menus = [];
        model.remindDay = [0, 1, 2, 3, 4, 5, 6, 7];
        model.jobTitles = [];
        model.jobGrades = [];
        model.permission = 0;

        model.getTargetType = function (isActive, excludeTargetTypeIds) {
            return commonFactory.getTargetType(isActive, excludeTargetTypeIds).success(function (targetTypes) {
                model.targetTypes = targetTypes;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getIndex = function (index, currentPage, pageSize) {
            var indexOfRow = pageSize * (currentPage - 1) + index + 1;
            return indexOfRow;
        };

        model.getSystemRoles = function (exceptNone) {
            return commonFactory.getSystemRoles(exceptNone).success(function (systemRoles) {
                model.systemRoles = systemRoles;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getKPI = function (kpiGroupId, isActive, isRequiredEvaluate) {
            return commonFactory.getKPI(kpiGroupId, isActive, isRequiredEvaluate).success(function (kpis) {
                model.kpis = kpis;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getKPIGroup = function (isActive) {
            return commonFactory.getKPIGroup(isActive).success(function (kpiGroups) {
                model.kpiGroups = kpiGroups;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAllGroup = function () {
            return commonFactory.getAllGroup().success(function (groups) {
                model.groups = groups;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAllEmployee = function (query, groupId, isSpecialUser, isDisabledUser, getByApprover, includeVerifier) {
            return commonFactory.getAllEmployee(query, groupId, isSpecialUser, isDisabledUser, getByApprover, includeVerifier).success(function (employees) {
                model.employees = employees;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getEmployeeByDepartmentId = function (departmentId) {
            return commonFactory.getEmployeeByDepartmentId(departmentId).success(function (employees) {
                model.employeesInDepartment = employees;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getEmployeeByGroupId = function (groupId) {
            return commonFactory.getEmployeeByGroupId(groupId).success(function (employees) {
                model.employeesInGroup = employees;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getEmployeeById = function (employeeId) {
            return commonFactory.getEmployeeById(employeeId).success(function (employee) {
                model.employee = employee;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        function createLabel(item, level) {
            var label = '',
                space = '',
                levelTemp = level;

            if (level > 0) {
                while (levelTemp > 0) {
                    label = label + " " + space;
                    space = String.fromCharCode(160) + String.fromCharCode(160) + String.fromCharCode(160);
                    levelTemp--;
                }

                label = label + ' ├─○ ' + item.Name;
            } else {
                label = item.Name;
            }

            return label;
        }

        function generateTreeDepartment(departments, options, level) {
            angular.forEach(departments, function (item) {
                options.push({
                    Name: createLabel(item, level),
                    DepartmentId: item.DepartmentId,
                    IsGroup: item.IsGroup
                });
                generateTreeDepartment(item.Childrens, options, level + 1);
            });

            return options;
        }

        model.generateTreeDepartment = function (departments) {
            return generateTreeDepartment(departments, [], 0);
        };

        model.getDepartment = function (parentId, isActive) {
            return commonFactory.getDepartment(parentId, isActive).success(function (data) {
                model.treeDepartment = data.TreeDepartment;
                model.departments = data.Departments;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAllStatus = function () {
            return commonFactory.getAllStatus().success(function (statusList) {
                model.statusList = statusList;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getPeriod = function (targetTypeId, isActive) {
            return commonFactory.getPeriod(targetTypeId, isActive).success(function (periods) {
                model.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getStandardGrade = function () {
            return commonFactory.getStandardGrade().success(function (standardGrades) {
                model.standardGrades = standardGrades;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAllPeriodIncludeImport = function () {
            return commonFactory.getAllPeriodIncludeImport().success(function (periods) {
                model.periodOptions = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getTargetTemplate = function (targetTypeId, isActive) {
            return commonFactory.getTargetTemplate(targetTypeId, isActive).success(function (targetTemplates) {
                model.targetTemplates = targetTemplates;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAllMonthlyPeriod = function () {
            return commonFactory.getAllMonthlyPeriod().success(function (periods) {
                model.monthlyPeriods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getMenusByEmployeeId = function () {
            return commonFactory.getMenusByEmployeeId().success(function (menus) {
                model.menus = menus;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAllJobTitle = function () {
            return commonFactory.getAllJobTitle().success(function (jobTitles) {
                model.jobTitles = jobTitles;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getUserPermissionOnFlowManagement = function () {
            return commonFactory.getUserPermissionOnFlowManagement().success(function (permission) {
                model.permission = parseInt(permission, 10);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getJobGrade = function (isActive) {
            return commonFactory.getJobGrade(isActive).success(function (jobGrades) {
                model.jobGrades = jobGrades;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        return model;
    }
})(angular);