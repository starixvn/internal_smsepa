'use strict';
/*global flowMngModule*/

flowMngModule.controller("FlowMngController",
['FlowMngModel', 'EmployeeModel',
function (FlowMngModel, EmployeeModel) {
    var vm = this;

    vm.selfs = FlowMngModel.selfs;
    vm.evaluators = FlowMngModel.evaluators;
    vm.reviewers = FlowMngModel.reviewers;
    vm.managements = FlowMngModel.managements;
    vm.humanResources = FlowMngModel.humanResources;

    vm.employees = EmployeeModel.employees;

    vm.initialize = function () {
        EmployeeModel.getAllEmployee().then(function () {
            vm.employees = EmployeeModel.employees;
        });
    };

    vm.initialize();
} ]);