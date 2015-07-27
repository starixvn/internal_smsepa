'use strict';
/*global flowMngModule*/

flowMngModule.controller("FlowMngController",
['FlowMngModel', 'EmployeeModel', 'CommonModel', 'utilitiesFactory', 'cacheService',
function (FlowMngModel, EmployeeModel, CommonModel, utilitiesFactory, cacheService) {
    var vm = this;

    vm.employees = EmployeeModel.employees;
    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;

    utilitiesFactory.getDateFormat(vm);

    vm.tabDepartmentSelected = function () {
        cacheService.setProperty(1);
    };

    vm.tabPersonalSelected = function () {
        cacheService.setProperty(2);
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