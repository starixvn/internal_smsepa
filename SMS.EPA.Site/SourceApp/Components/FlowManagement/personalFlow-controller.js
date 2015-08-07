'use strict';
/*global flowMngModule*/

flowMngModule.controller("PersonalFlowController",
['$scope', '$routeParams', 'FlowMngModel', 'CommonModel', 'cacheService',
function ($scope, $routeParams, FlowMngModel, CommonModel, cacheService) {
    var vm = this;

    vm.FlowTemplateId = parseInt($routeParams.id);

    vm.selfs = FlowMngModel.selfs;
    vm.evaluators = FlowMngModel.evaluators;
    vm.reviewers = FlowMngModel.reviewers;
    vm.managements = FlowMngModel.managements;
    vm.humanResources = FlowMngModel.humanResources;

    vm.employees = [];
    vm.showListEmployee = true;
    vm.department = null;
    vm.showDeleteButton = false;
    vm.defaultFlowSettings = [];
    vm.personalFlowSettings = [];

    /// Show or hide list Employee.
    vm.toggleDepartmentDraggableList = function () {
        vm.showListEmployee = !vm.showListEmployee;
    };

    /// Remove item from list.
    vm.removeItemFromList = function (item, list) {
        FlowMngModel.removeItemFromList(item, list);
    };

    vm.getEmployee = function () {
        if (vm.departmentId > 0) {
            CommonModel.getEmployeeByDepartmentId(vm.departmentId).then(function () {
                vm.employees = CommonModel.employeesInDepartment;
            });
        }
        else if (vm.departmentId < 0) {
            CommonModel.getEmployeeByGroupId(Math.abs(vm.departmentId)).then(function () {
                vm.employees = CommonModel.employeesInGroup;
            });
        }
    };

    vm.onSelectedDepartment = function () {
        if (vm.department.DepartmentId > 0) {
            FlowMngModel.getEmployeeFlowSetting(vm.FlowTemplateId, vm.department.DepartmentId, null).then(function () {
                vm.defaultFlowSettings = FlowMngModel.defaultFlowSettings;
                vm.personalFlowSettings = FlowMngModel.personalFlowSettings;
            });
        }
        else if (vm.department.DepartmentId < 0) {
            FlowMngModel.getEmployeeFlowSetting(vm.FlowTemplateId, null, Math.abs(vm.department.DepartmentId)).then(function () {
                vm.defaultFlowSettings = FlowMngModel.defaultFlowSettings;
                vm.personalFlowSettings = FlowMngModel.personalFlowSettings;
            });
        }

        FlowMngModel.resetCurrentFlow(vm);
        vm.defaultSelect = null;
        vm.personalSelect = null;
        vm.showDeleteButton = false;
    };

    vm.moveItem = function (item, from, to) {
        $.each(from, function (i) {
            var currentItem = from[i];

            if (currentItem.EmployeeId === parseInt(item)) {
                from.splice(i, 1);
                to.push(currentItem);

                return false;
            }
        });
    };

    vm.moveAll = function (from, to) {
        $.each(from, function (i) {
            to.push(from[i]);
        });

        from.length = 0;
    };

    var getEmployeeById = function (sender, employeeId, position) {
        CommonModel.getEmployeeById(parseInt(employeeId)).then(function () {
            FlowMngModel.addEmployee(sender, vm, CommonModel.employee, position);
        });
    };

    vm.getPersonalFlow = function (item) {
        var employeeId = parseInt(item[0]);
        vm.employeeId = employeeId;

        FlowMngModel.getPersonalFlow(employeeId, vm.FlowTemplateId).then(function () {
            vm.selfs = FlowMngModel.selfs;
            vm.evaluators = FlowMngModel.evaluators;
            vm.reviewers = FlowMngModel.reviewers;
            vm.managements = FlowMngModel.managements;
            vm.humanResources = FlowMngModel.humanResources;

            if (vm.selfs.employees.length > 0) {
                vm.showDeleteButton = true;
            }
            else {
                CommonModel.getEmployeeById(employeeId).then(function () {
                    vm.selfs.employees.push(CommonModel.employee);
                });

                vm.showDeleteButton = false;
            }
        });
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

    // On Created list after Drag & Drop.
    $scope.$on('list-created', function (ev, val) {
        getEmployeeById(val.sender, val.id, val.to);
    });

    $scope.$watch(function () {
        return cacheService.getProperty();
    }, function (value) {
        if (value === 2) {
            CommonModel.getAllEmployee().then(function () {
                vm.employees = CommonModel.employees;
            });

            FlowMngModel.resetCurrentFlow(vm);
            vm.showListEmployee = true;
            vm.showDeleteButton = false;
            vm.department = null;
            vm.departmentId = null;
            vm.defaultFlowSettings = [];
            vm.personalFlowSettings = [];
        }
    });

    vm.savePersonalFlow = function () {
        if (FlowMngModel.validatePersonalFlow(vm)) {
            FlowMngModel.createPersonalFlow(vm);
        }
    };

    vm.deletePersonalFlow = function () {
        FlowMngModel.openPopupDeletePersonalFlow(vm);
    };
} ]);