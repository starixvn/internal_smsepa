'use strict';
/*global targetEmployeeSettingModule*/

targetEmployeeSettingModule.controller('TargetSettingTemplateController',
['TargetEmployeeSettingModel', 'enumerationsFactory',
function (TargetEmployeeSettingModel, enumerationsFactory) {
    var vm = this;
    vm.permission = TargetEmployeeSettingModel.permission;

    vm.policyEnum = enumerationsFactory.policy;

    vm.dataTab = {
        department: true,
        personnel: false
    };

    vm.tabDepartmentSelect = function () {
        vm.dataTab.department = true;
        vm.dataTab.personnel = false;
    };

    vm.tabpersonnelSelect = function () {
        vm.dataTab.department = false;
        vm.dataTab.personnel = true;
    };

    TargetEmployeeSettingModel.getPermissionOnTargetEmployeeSetting().then(function () {
        vm.permission = TargetEmployeeSettingModel.permission;
    });
}]);

targetEmployeeSettingModule.controller('TargetPersonnelSettingController',
['$scope', 'TargetEmployeeSettingModel', 'EmployeeModel', 'CommonModel', 'TargetTemplateModel', 'utilitiesFactory',
function ($scope, TargetEmployeeSettingModel, EmployeeModel, CommonModel, TargetTemplateModel, utilitiesFactory) {
    var vm = this;

    vm.targetEmployeeSettings = TargetEmployeeSettingModel.targetEmployeeSettings;
    vm.targetTypes = CommonModel.targetTypes;
    vm.targetTemplates = TargetTemplateModel.targetTemplates;
    vm.isFirstLoad = true;

    vm.filter = {
        groupId: null,
        targetTypeId: null,
        employee: null,
        showAll: null
    };

    vm.targetEmployeeSetting = {
        targetTemplateId: null,
        employeeIds: ''
    };

    vm.selectedAll = false;
    vm.groupOptions = [];
    vm.currentPage = 1;
    vm.pageSize = 15;

    utilitiesFactory.numberEntryList(vm);

    var getTargetEmployeeSetting = function (groupId, employeeId, targetTypeId, showAll) {
        TargetEmployeeSettingModel.getTargetEmployeeSetting(groupId, employeeId, targetTypeId, showAll).then(function () {
            vm.targetEmployeeSettings = TargetEmployeeSettingModel.targetEmployeeSettings;
            utilitiesFactory.searchOnPage(vm, vm.targetEmployeeSettings);
            vm.isFirstLoad = false;
        });
    };

    $scope.$watch(function () { return $scope.targetCtrl.dataTab.personnel; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                CommonModel.getAllGroup().then(function () {
                    vm.groupOptions = CommonModel.groups;
                });

                CommonModel.getTargetType().then(function () {
                    vm.targetTypes = CommonModel.targetTypes;
                });

                TargetTemplateModel.getTargetTemplate(null, true).then(function () {
                    vm.targetTemplates = TargetTemplateModel.targetTemplates;

                    if (vm.targetTemplates && vm.targetTemplates.length > 0) {
                        vm.targetEmployeeSetting.targetTemplateId = vm.targetTemplates[0].TargetTemplateId;
                    }
                });

                getTargetEmployeeSetting(null, null, null, null);
            }
        });

    vm.getEmployee = function () {
        return EmployeeModel.getAllEmployee(vm.searchEmployee);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchEmployee.trim() === '') {
            if (vm.filter.employee) {
                vm.filter.employee = null;
            }
        }
    };

    vm.query = function (filter) {
        var employeeId = null;
        vm.searchKey = '';
        vm.currentPage = 1;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        getTargetEmployeeSetting(filter.groupId, employeeId, filter.targetTypeId, filter.showAll);
    };

    var getSelectedEmployees = function (targetEmployeeSettings) {
        var employeeIds = '';

        angular.forEach(targetEmployeeSettings, function (targetEmployee) {
            if (targetEmployee.Selected) {
                employeeIds += employeeIds ? ',' + targetEmployee.Employee.EmployeeId : targetEmployee.Employee.EmployeeId;
            }
        });

        return employeeIds;
    };

    vm.assignTargetTemplate = function () {
        vm.targetEmployeeSetting.employeeIds = getSelectedEmployees(vm.targetEmployeeSettings);

        TargetEmployeeSettingModel.assignTargetTemplateToEmployee(vm.targetEmployeeSetting).then(function () {
            vm.query(vm.filter);
            vm.selectedAll = false;
        });
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.filteredItems, function (targetEmployee) {
            targetEmployee.Selected = selectedAll;
        });

        angular.forEach(vm.targetEmployeeSettings, function (targetEmployee) {
            targetEmployee.Selected = selectedAll;
        });
    };

    vm.syncSelectedEmployee = function (employeeId, selected) {
        angular.forEach(vm.targetEmployeeSettings, function (targetEmployee) {
            if (targetEmployee.Employee.EmployeeId === employeeId) {
                angular.forEach(vm.filteredItems, function (filterItem) {
                    if (targetEmployee.Employee.EmployeeId === filterItem.Employee.EmployeeId) {
                        filterItem.Selected = selected;
                        targetEmployee.Selected = selected;
                    }
                });
            }
        });
    };

    vm.deleteTargetTemplate = function (employee, targetTemplate) {
        TargetEmployeeSettingModel.openPopupDeleteTargetEmployeeSetting(targetTemplate, employee);
    };
}]);

targetEmployeeSettingModule.controller('TargetDepartmentSettingController',
['$scope', 'TargetEmployeeSettingModel', 'CommonModel', 'TargetTemplateModel', 'utilitiesFactory',
function ($scope, TargetEmployeeSettingModel, CommonModel, TargetTemplateModel, utilitiesFactory) {
    var vm = this;

    vm.targetDepartmentSettings = TargetEmployeeSettingModel.targetDepartmentSettings;
    vm.targetTemplates = TargetTemplateModel.targetTemplates;
    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;
    vm.targetTypes = CommonModel.targetTypes;
    vm.isFirstLoad = true;

    vm.filter = {
        departmentId: null,
        isGroup: null,
        targetTypeId: null,
        showAll: null
    };
    vm.targetDepartmentSetting = {
        targetTemplateId: null,
        departmentSettings: []
    };

    vm.selectedAll = false;
    vm.currentPage = 1;
    vm.pageSize = 15;

    utilitiesFactory.numberEntryList(vm);

    var getTargetDepartmentSetting = function (departmentId, isGroup, targetTypeId, showAll) {
        TargetEmployeeSettingModel.getTargetDepartmentSetting(departmentId, isGroup, targetTypeId, showAll).then(function () {
            vm.targetDepartmentSettings = TargetEmployeeSettingModel.targetDepartmentSettings;
            utilitiesFactory.searchOnPage(vm, vm.targetDepartmentSettings);
            vm.isFirstLoad = false;
        });
    };

    $scope.$watch(function () { return $scope.targetCtrl.dataTab.department; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                CommonModel.getDepartment(null, true).then(function () {
                    vm.departments = CommonModel.departments;
                    vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
                });

                CommonModel.getTargetType().then(function () {
                    vm.targetTypes = CommonModel.targetTypes;
                });

                TargetTemplateModel.getTargetTemplate(null, true).then(function () {
                    vm.targetTemplates = TargetTemplateModel.targetTemplates;

                    if (vm.targetTemplates && vm.targetTemplates.length > 0) {
                        vm.targetDepartmentSetting.targetTemplateId = vm.targetTemplates[0].TargetTemplateId;
                    }
                });

                getTargetDepartmentSetting(vm.filter.departmentId, null, vm.filter.targetTypeId, vm.filter.showAll);
            }
        });

    vm.updateIsGroup = function (departmentId) {
        if (departmentId) {
            vm.filter.isGroup = departmentId < 0 ? true : false;
        }
        else {
            vm.filter.isGroup = null;
        }
    };

    vm.query = function (filter) {
        var departmentId = null;
        vm.searchKey = '';
        vm.currentPage = 1;

        if (filter.departmentId) {
            departmentId = filter.departmentId < 0 ? -filter.departmentId : filter.departmentId;
        }

        getTargetDepartmentSetting(departmentId, filter.isGroup, filter.targetTypeId, filter.showAll);
    };

    var getSelectedDepartments = function (targetDepartmentSettings) {
        var departments = [];

        angular.forEach(targetDepartmentSettings, function (targetDepartment) {
            if (targetDepartment.Selected) {
                var department = {
                    DepartmentId: targetDepartment.DepartmentId,
                    IsGroup: targetDepartment.IsGroup
                };
                departments.push(department);
            }
        });

        return departments;
    };

    vm.assignTargetTemplate = function () {
        vm.targetDepartmentSetting.departmentSettings = getSelectedDepartments(vm.targetDepartmentSettings);

        TargetEmployeeSettingModel.assignTargetTemplateToDepartment(vm.targetDepartmentSetting).then(function () {
            vm.query(vm.filter);
            vm.selectedAll = false;
        });
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };

    vm.checkAll = function (selectedAll) {
        angular.forEach(vm.filteredItems, function (targetDepartment) {
            targetDepartment.Selected = selectedAll;
        });

        angular.forEach(vm.targetDepartmentSettings, function (targetDepartment) {
            targetDepartment.Selected = selectedAll;
        });
    };

    vm.syncSelectedDepartment = function (department, selected) {
        angular.forEach(vm.targetDepartmentSettings, function (targetDepartment) {
            if (targetDepartment.DepartmentId === department.DepartmentId
                    && targetDepartment.IsGroup === department.IsGroup) {
                angular.forEach(vm.filteredItems, function (filterItem) {
                    if (targetDepartment.DepartmentId === filterItem.DepartmentId
                            && targetDepartment.IsGroup === filterItem.IsGroup) {
                        filterItem.Selected = selected;
                        targetDepartment.Selected = selected;
                    }
                });
            }
        });
    };

    vm.deleteTargetTemplate = function (department, targetTemplate) {
        TargetEmployeeSettingModel.openPopupDeleteTargetDepartmentSetting(targetTemplate, department);
    };
}]);