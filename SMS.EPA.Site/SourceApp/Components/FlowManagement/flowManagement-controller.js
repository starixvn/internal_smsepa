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

    var createLabel = function (item, level) {
        var label = '',
			space = '',
			levelTemp = level;

        if (level > 0) {
            while (levelTemp > 0) {
                label = label + " " + space;
                space = String.fromCharCode(160) + String.fromCharCode(160) + String.fromCharCode(160);
                levelTemp--;
            }

            label = label + item.Name;
        }
        else {
            label = item.Name;
        }

        return label;
    };

    var generateTreeDepartment = function (departments, options, level) {
        angular.forEach(departments, function (item) {
            options.push({ Name: createLabel(item, level), DepartmentId: item.DepartmentId });
            generateTreeDepartment(item.Childrens, options, level + 1);
        });

        return options;
    };

    

    vm.initialize = function () {
        EmployeeModel.getAllEmployee().then(function () {
            vm.employees = EmployeeModel.employees;
        });

        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = generateTreeDepartment(CommonModel.treeDepartment.Childrens, [], 0);
        });

        FlowMngModel.resetCurrentFlow(FlowMngModel);
    };

    vm.initialize();
} ]);