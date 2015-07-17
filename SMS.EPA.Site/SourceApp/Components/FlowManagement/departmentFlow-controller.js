'use strict';
/*global flowMngModule*/

flowMngModule.controller("DepartmentFlowController",
['FlowMngModel',
function (FlowMngModel) {
    var vm = this;

    vm.selfs = FlowMngModel.selfs;
    vm.evaluators = FlowMngModel.evaluators;
    vm.reviewers = FlowMngModel.reviewers;
    vm.managements = FlowMngModel.managements;
    vm.humanResources = FlowMngModel.humanResources;

    vm.showListEmployee = true;

    /// Show or hide list Employee.
    vm.toggleDepartmentDraggableList = function () {
        vm.showListEmployee = !vm.showListEmployee;
    };

    vm.initialize = function () {
    };

    vm.initialize();
} ]);