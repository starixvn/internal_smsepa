'use strict';
/*global flowMngModule*/

flowMngModule.controller("DepartmentFlowController",
['$scope', 'FlowMngModel', 'CommonModel',
function ($scope, FlowMngModel, CommonModel) {
    var vm = this;

    vm.selfs = FlowMngModel.selfs;
    vm.evaluators = FlowMngModel.evaluators;
    vm.reviewers = FlowMngModel.reviewers;
    vm.managements = FlowMngModel.managements;
    vm.humanResources = FlowMngModel.humanResources;

    vm.showListEmployee = true;

    FlowMngModel.calendar(vm);

    /// Open calendar.
    vm.openPopupStartDate = function ($event, obj) {
        $event.preventDefault();
        $event.stopPropagation();
        FlowMngModel.resetOpenCalendar();

        obj.startDateOpened = true;
    };

    vm.openPopupEndDate = function ($event, obj) {
        $event.preventDefault();
        $event.stopPropagation();
        FlowMngModel.resetOpenCalendar();

        obj.endDateOpened = true;
    };

    /// Show or hide list Employee.
    vm.toggleDepartmentDraggableList = function () {
        vm.showListEmployee = !vm.showListEmployee;
    };

    vm.onSelectedDepartment = function () {
        vm.flowType = vm.department.DepartmentId;
        FlowMngModel.getFlow(vm.department, vm.flowType).then(function () {
            vm.selfs = FlowMngModel.selfs;
            vm.evaluators = FlowMngModel.evaluators;
            vm.reviewers = FlowMngModel.reviewers;
            vm.managements = FlowMngModel.managements;
            vm.humanResources = FlowMngModel.humanResources;
        });
    };

    /// Remove item from list.
    vm.removeItemFromList = function (item, list) {
        FlowMngModel.removeItemFromList(item, list);
    };

    // Sort item in list.
    $scope.$on('list-sorted', function (ev, val) {
        var sender = val.sender;
        switch (sender) {
            case "selfList":
                vm.selfs.employees.splice(val.to, 0, vm.selfs.employees.splice(val.from, 1)[0]);
                break;
            case "evaluatorList":
                vm.evaluators.employees.splice(val.to, 0, vm.evaluators.employees.splice(val.from, 1)[0]);
                break;
            case "reviewerList":
                vm.reviewers.employees.splice(val.to, 0, vm.reviewers.employees.splice(val.from, 1)[0]);
                break;
            case "managementList":
                vm.managements.employees.splice(val.to, 0, vm.managements.employees.splice(val.from, 1)[0]);
                break;
            case "humanResourceList":
                vm.humanResources.employees.splice(val.to, 0, vm.humanResources.employees.splice(val.from, 1)[0]);
                break;
        }
    });

    var getEmployeeById = function (sender, employeeId, position) {
        CommonModel.getEmployeeById(parseInt(employeeId)).then(function () {
            FlowMngModel.addEmployee(sender, vm, CommonModel.employee, position);
        });
    };

    // On Created list after Drag & Drop.
    $scope.$on('list-created', function (ev, val) {
        getEmployeeById(val.sender, val.id, val.to);
    });

    vm.confirmIncludeSelfEvaluation = function (flow) {
        if (vm.department && vm.department.DepartmentId) {
            FlowMngModel.openPopupConfirmIncludeSelfEvaluation(flow);
        }
    };

    vm.saveFlow = function () {
        FlowMngModel.createFlow(vm);
    };

    vm.deleteFlow = function () {
        FlowMngModel.openPopupDeleteFlow(vm.department, vm.flowType);
    };

    vm.initialize = function () {
    };

    vm.initialize();
} ]);