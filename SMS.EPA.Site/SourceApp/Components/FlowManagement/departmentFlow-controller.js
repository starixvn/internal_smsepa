'use strict';
/*global flowMngModule*/

flowMngModule.controller("DepartmentFlowController",
['$scope', 'FlowMngModel', 'CommonModel', 'utilitiesFactory',
function ($scope, FlowMngModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.selfs = FlowMngModel.selfs;
    vm.evaluators = FlowMngModel.evaluators;
    vm.reviewers = FlowMngModel.reviewers;
    vm.managements = FlowMngModel.managements;
    vm.humanResources = FlowMngModel.humanResources;

    vm.showListEmployee = true;

    utilitiesFactory.getDateFormat(vm);
    vm.calendar = {
        selfs: {
            startDateOpened: false,
            endDateOpened: false
        },
        evaluators: {
            startDateOpened: false,
            endDateOpened: false
        },
        reviewers: {
            startDateOpened: false,
            endDateOpened: false
        },
        managements: {
            startDateOpened: false,
            endDateOpened: false
        },
        humanResources: {
            startDateOpened: false,
            endDateOpened: false
        }
    };

    var resetOpenCalendar = function () {
        vm.calendar.selfs.startDateOpened = false;
        vm.calendar.selfs.endDateOpened = false;

        vm.calendar.evaluators.startDateOpened = false;
        vm.calendar.evaluators.endDateOpened = false;

        vm.calendar.reviewers.startDateOpened = false;
        vm.calendar.reviewers.endDateOpened = false;

        vm.calendar.managements.startDateOpened = false;
        vm.calendar.managements.endDateOpened = false;

        vm.calendar.humanResources.startDateOpened = false;
        vm.calendar.humanResources.endDateOpened = false;
    };

    /// Open calendar.
    vm.openPopupStartDate = function ($event, obj) {
        $event.preventDefault();
        $event.stopPropagation();
        resetOpenCalendar();

        obj.startDateOpened = true;
    };

    vm.openPopupEndDate = function ($event, obj) {
        $event.preventDefault();
        $event.stopPropagation();
        resetOpenCalendar();

        obj.endDateOpened = true;
    };

    /// Show or hide list Employee.
    vm.toggleDepartmentDraggableList = function () {
        vm.showListEmployee = !vm.showListEmployee;
    };

    vm.onSelectedDepartment = function () {
        vm.flowType = vm.departmentId;
        FlowMngModel.getFlow(vm.departmentId, vm.flowType).then(function () {
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

    vm.saveFlow = function () {
        FlowMngModel.createFlow(vm);
    };

    vm.deleteFlow = function () {
    };

    vm.initialize = function () {
    };

    vm.initialize();
} ]);