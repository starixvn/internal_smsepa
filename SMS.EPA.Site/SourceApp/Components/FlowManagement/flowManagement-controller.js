'use strict';
/*global flowMngModule*/

flowMngModule.controller("FlowMngController",
['FlowMngModel', 'EmployeeModel', 'CommonModel', 'utilitiesFactory',
function (FlowMngModel, EmployeeModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.employees = EmployeeModel.employees;
    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;

    utilitiesFactory.getDateFormat(vm);

    vm.tabSelected = function () {
        FlowMngModel.resetCurrentData();
    };

    vm.initialize = function () {
        EmployeeModel.getAllEmployee().then(function () {
            vm.employees = EmployeeModel.employees;
        });

        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = CommonModel.treeDepartment;
        });

        FlowMngModel.resetCurrentFlow(FlowMngModel);
    };

    vm.initialize();
} ]);