'use strict';
/*global checkFlowAssignmentModule*/

checkFlowAssignmentModule.controller('CheckFlowAssignmentController',
['$scope', 'CheckFlowModel', 'EmployeeModel', 'CommonModel', 'utilitiesFactory', 'TargetTemplateModel',
function ($scope, CheckFlowModel, EmployeeModel, CommonModel, utilitiesFactory, TargetTemplateModel) {
    var vm = this;

    /*
    ** Variable setting.
    */
    vm.employeeSettings = CheckFlowModel.employeeSettings;
    vm.targetTemplates = TargetTemplateModel.targetTemplates;
    vm.employees = [];

    vm.filter = {
        groupId: null,
        employeeId: null,
        targetTypeId: null,
        employee: null
    };

    vm.filterAssign = {
        evaluator : { 
            ids: null,
            startDate: new Date(),
            endDate: new Date()
        },
        reviewer: {
            ids: null,
            startDate: new Date(),
            endDate: new Date()
        },
        management: {
            ids: null,
            startDate: new Date(),
            endDate: new Date()
        },
        targetTemplateId: null,
        evaluatorEmployees: [],
        reviewerEmployees: [],
        managementEmployees: []
    };

    vm.selectedAll = false;
    vm.groupOptions = [];
    vm.currentPage = 1;
    vm.pageSize = 15;

    /*
    ** Calendar setting.
    */
    vm.calendar = {
        evaluator: {
            startDateOpened: false,
            endDateOpened: false
        },
        reviewer: {
            startDateOpened: false,
            endDateOpened: false
        },
        management: {
            startDateOpened: false,
            endDateOpened: false
        }
    };
    vm.evaluatorType = CheckFlowModel.evaluatorType;

    vm.openPopupStartDate = function (event, evaluatorType) {
        CheckFlowModel.openPopupStartDate(vm, event, evaluatorType);
    };

    vm.openPopupEndDate = function (event, evaluatorType) {
        CheckFlowModel.openPopupEndDate(vm, event, evaluatorType);
    };

    /*
    ** Get data to filter and setting information.
    */
    utilitiesFactory.numberEntryList(vm);
    utilitiesFactory.getDateFormat(vm);

    EmployeeModel.getAllGroup().then(function () {
        vm.groupOptions = EmployeeModel.groups;
    });

    CommonModel.getTargetType().then(function () {
        vm.targetTypes = CommonModel.targetTypes;
    });

    TargetTemplateModel.getTargetTemplate(null, true).then(function () {
        vm.targetTemplates = TargetTemplateModel.targetTemplates;
    });

    /*
    ** Search employee auto complete.
    */
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

    /*
    ** Get employees flow and target setting.
    */
    EmployeeModel.getAllEmployee().then(function () {
        vm.employees = EmployeeModel.employees;
        vm.filterAssign.evaluatorEmployees = angular.copy(vm.employees);
        vm.filterAssign.reviewerEmployees = angular.copy(vm.employees);
        vm.filterAssign.managementEmployees = angular.copy(vm.employees);
    });

    vm.updateTrigger = function () {
        $scope.$broadcast('evaluatorChosen');
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