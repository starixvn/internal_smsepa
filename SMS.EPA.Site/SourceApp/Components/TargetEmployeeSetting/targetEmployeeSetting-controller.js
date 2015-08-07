'use strict';
/*global targetEmployeeSettingModule*/

targetEmployeSettingModule.controller('TargetSettingTemplateController',
['TargetEmployeeSettingModel',
function (TargetEmployeeSettingModel) {
    var vm = this;

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
}]);

targetEmployeeSettingModule.controller('TargetEmployeeSettingController',
['TargetEmployeeSettingModel', 'EmployeeModel', 'CommonModel', 'TargetTemplateModel', 'utilitiesFactory',
function (TargetEmployeeSettingModel, EmployeeModel, CommonModel, TargetTemplateModel, utilitiesFactory) {
    var vm = this;

    vm.targetEmployeeSettings = TargetEmployeeSettingModel.targetEmployeeSettings;
    vm.targetTypes = CommonModel.targetTypes;
    vm.targetTemplates = TargetTemplateModel.targetTemplates;

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

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    CommonModel.getTargetType().then(function () {
        vm.targetTypes = CommonModel.targetTypes;
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

    TargetTemplateModel.getTargetTemplate(null, true).then(function () {
        vm.targetTemplates = TargetTemplateModel.targetTemplates;
    });

    var getTargetEmployeeSetting = function (groupId, employeeId, targetTypeId, showAll) {
        TargetEmployeeSettingModel.getTargetEmployeeSetting(groupId, employeeId, targetTypeId, showAll).then(function () {
            vm.targetEmployeeSettings = TargetEmployeeSettingModel.targetEmployeeSettings;
            utilitiesFactory.searchOnPage(vm, vm.targetEmployeeSettings);
        });
    };

    getTargetEmployeeSetting(null, null, null, null);

    vm.query = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        getTargetEmployeeSetting(filter.groupId, employeeId, filter.targetTypeId, filter.showAll);
    };

    var getSelectedEmployees = function (targetEmployeeSettings) {
        var employeeIds = '';

        angular.forEach(targetEmployeeSettings, function (targetEmployee) {
            if (targetEmployee.Selected) {
                employeeIds += employeeIds ? ',' + targetEmployee.EmployeeId : targetEmployee.EmployeeId;
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
            if (targetEmployee.EmployeeId === employeeId) {
                angular.forEach(vm.filteredItems, function (filterItem) {
                    if (targetEmployee.EmployeeId === filterItem.EmployeeId ) {
                        filterItem.Selected = selected;
                        targetEmployee.Selected = selected;
                    }
                });
             }
        });
    };
}]);