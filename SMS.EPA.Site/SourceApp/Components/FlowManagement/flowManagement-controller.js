'use strict';
/*global flowMngModule*/

flowMngModule.controller("FlowMngController",
['FlowMngModel', 'EmployeeModel', 'CommonModel', 'utilitiesFactory', 'cacheService',
function (FlowMngModel, EmployeeModel, CommonModel, utilitiesFactory, cacheService) {
    var vm = this;

    vm.employees = EmployeeModel.employees;
    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;

    vm.targetTypes = CommonModel.targetTypes;
    vm.flowTemplates = FlowMngModel.flowTemplates;

    utilitiesFactory.getDateFormat(vm);

    vm.filter = {
        targetTypeId: '',
        isActive: 'true'
    };

    utilitiesFactory.numberEntryList(vm);
    vm.pageSize = 15;
    vm.currentPage = 1;

    var getFlowTemplate = function (targetTypeId, isActive) {
        FlowMngModel.getFlowTemplate(targetTypeId, isActive).then(function () {
            vm.flowTemplates = FlowMngModel.flowTemplates;
            utilitiesFactory.searchOnPage(vm, vm.flowTemplates);
        });
    };

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

            label = label + ' ├─○ ' + item.Name;
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

    vm.query = function (filter) {
        getFlowTemplate(filter.targetTypeId, filter.isActive);
    };

    vm.initialize = function () {
        EmployeeModel.getAllEmployee().then(function () {
            vm.employees = EmployeeModel.employees;
        });

        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = generateTreeDepartment(CommonModel.treeDepartment.Childrens, [], 0);
        });

        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        FlowMngModel.resetCurrentFlow(FlowMngModel);

        getFlowTemplate(vm.filter.targetTypeId, vm.filter.isActive);
    };

    vm.initialize();

    /// Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
} ]);