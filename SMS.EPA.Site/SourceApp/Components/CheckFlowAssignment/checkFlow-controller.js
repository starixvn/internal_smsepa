'use strict';
/*global checkFlowAssignmentModule*/

checkFlowAssignmentModule.controller('CheckFlowAssignmentController',
['CheckFlowModel', 'EmployeeModel', 'CommonModel', 'utilitiesFactory',
function (CheckFlowModel, EmployeeModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.employeeSettings = CheckFlowModel.employeeSettings;
    vm.filter = {
        groupId: null,
        employeeId: null,
        targetTypeId: null,
        employee: null
    };

    vm.selectedAll = false;
    vm.groupOptions = [];
    vm.currentPage = 1;
    vm.pageSize = 15;

    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

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

    var getEmployeeSettings = function (groupId, employeeId, targetTypeId) {
        CheckFlowModel.getEmployeeFlowTargetSetting(groupId, employeeId, targetTypeId).then(function () {
            vm.employeeSettings = CheckFlowModel.employeeSettings;
            utilitiesFactory.searchOnPage(vm, vm.employeeSettings);
        });
    };

    getEmployeeSettings(vm.filter.groupId, vm.filter.employeeId, vm.filter.targetTypeId);

    vm.query = function (filter) {
        var employeeId = null;

        if (filter.employee) {
            employeeId = filter.employee.EmployeeId;
        }

        getEmployeeSettings(filter.groupId, employeeId, filter.targetTypeId);
    };

    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
}]);