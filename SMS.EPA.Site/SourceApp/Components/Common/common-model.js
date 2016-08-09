'use strict';
/*global commonModule*/

commonModule.factory('CommonModel',
['$modal', 'commonFactory', 'errorFactory',
function ($modal, commonFactory, errorFactory) {
    var commonModel = {};

    commonModel.targetTypes = [];
    commonModel.systemRoles = [];
    commonModel.kpis = [];
    commonModel.kpiGroups = [];
    commonModel.groups = [];
    commonModel.employees = [];
    commonModel.employeesInDepartment = [];
    commonModel.employeesInGroup = [];
    commonModel.employee = {};
    commonModel.departments = [];
    commonModel.treeDepartment = {};
    commonModel.statusList = [];
    commonModel.periods = [];
    commonModel.monthlyPeriods = [];
    commonModel.standardGrades = [];
    commonModel.periodOptions = [];
    commonModel.otherViolationDetails = [];
    commonModel.targetTemplates = [];
    commonModel.menus = [];
    commonModel.remindDay = [0, 1, 2, 3, 4, 5, 6, 7];
    commonModel.isOnNormalPdfPage = true;

    /// Get target type.
    commonModel.getTargetType = function (isActive, excludeTargetTypeIds) {
        return commonFactory.getTargetType(isActive, excludeTargetTypeIds)
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
    commonModel.getAllEmployee = function (query, groupId, isSpecialUser, isDisabledUser, getByApprover, includeVerifier) {
        return commonFactory.getAllEmployee(query, groupId, isSpecialUser, isDisabledUser, getByApprover, includeVerifier)
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
            commonModel.employeesInDepartment = employees;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get employee by group id.
    commonModel.getEmployeeByGroupId = function (groupId) {
        return commonFactory.getEmployeeByGroupId(groupId)
        .success(function (employees) {
            commonModel.employeesInGroup = employees;
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

            label = label + ' ├─○ ' + item.Name;
        }
        else {
            label = item.Name;
        }

        return label;
    };

    var generateTreeDepartment = function (departments, options, level) {
        angular.forEach(departments, function (item) {
            options.push({
                Name: createLabel(item, level),
                DepartmentId: item.DepartmentId,
                IsGroup: item.IsGroup
            });
            generateTreeDepartment(item.Childrens, options, level + 1);
        });

        return options;
    };

    commonModel.generateTreeDepartment = function (departments) {
        return generateTreeDepartment(departments, [], 0);
    };

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

    commonModel.getPeriod = function (targetTypeId, isActive) {
        return commonFactory.getPeriod(targetTypeId, isActive)
        .success(function (periods) {
            commonModel.periods = periods;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    commonModel.getStandardGrade = function () {
        return commonFactory.getStandardGrade()
        .success(function (standardGrades) {
            commonModel.standardGrades = standardGrades;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    commonModel.getAllPeriodIncludeImport = function () {
        return commonFactory.getAllPeriodIncludeImport().success(function (periods) {
            commonModel.periodOptions = periods;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    commonModel.getOtherViolationDetailByEPAId = function (epaId) {
        return commonFactory.getOtherViolationDetailByEPAId(epaId)
        .success(function (otherViolationDetails) {
            commonModel.otherViolationDetails = otherViolationDetails;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    commonModel.openPopupViewOtherViolationDetail = function (epaId) {
        var modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/Common/Templates/modalViewOtherViolationDetail.html',
            controller: "OtherViolationDetailController as otherViolationCtrl",
            backdrop: 'static',
            resolve: {
                epaId: function () {
                    return epaId;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return modalInstance.result;
    };

    commonModel.getTargetTemplate = function (targetTypeId, isActive) {
        return commonFactory.getTargetTemplate(targetTypeId, isActive)
        .success(function (targetTemplates) {
            commonModel.targetTemplates = targetTemplates;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    commonModel.displayOnNormalWidthPage = function (epaId) {
        return commonFactory.displayOnNormalWidthPage(epaId)
        .success(function (isOnNormalPdfPage) {
            commonModel.isOnNormalPdfPage = isOnNormalPdfPage;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    commonModel.getAllMonthlyPeriod = function () {
        return commonFactory.getAllMonthlyPeriod()
        .success(function (periods) {
            commonModel.monthlyPeriods = periods;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    commonModel.getMenusByEmployeeId = function () {
        return commonFactory.getMenusByEmployeeId()
        .success(function (menus) {
            commonModel.menus = menus;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return commonModel;
}]);