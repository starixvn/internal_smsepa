'use strict';
/*global flowMngModule*/

flowMngModule.controller("DepartmentFlowController",
['$scope', '$window', '$routeParams', 'FlowMngModel', 'CommonModel', 'cacheService',
function ($scope, $window, $routeParams, FlowMngModel, CommonModel, cacheService) {
    var vm = this;

    vm.FlowTemplateId = parseInt($routeParams.id);

    vm.selfs = FlowMngModel.selfs;
    vm.evaluators = FlowMngModel.evaluators;
    vm.reviewers = FlowMngModel.reviewers;
    vm.managements = FlowMngModel.managements;
    vm.humanResources = FlowMngModel.humanResources;

    vm.employees = [];
    vm.showListEmployee = true;
    vm.showDeleteButton = false;
    vm.department = null;
    vm.departmentId = null;

    /// Show or hide list Employee.
    vm.toggleDepartmentDraggableList = function () {
        vm.showListEmployee = !vm.showListEmployee;
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

    vm.getDepartmentFlow = function () {
        FlowMngModel.getDepartmentFlow(vm.department, vm.FlowTemplateId).then(function () {
            vm.selfs = FlowMngModel.selfs;
            vm.evaluators = FlowMngModel.evaluators;
            vm.reviewers = FlowMngModel.reviewers;
            vm.managements = FlowMngModel.managements;
            vm.humanResources = FlowMngModel.humanResources;

            if (vm.evaluators.employees.length > 0 ||
                vm.reviewers.employees.length > 0 ||
                vm.managements.employees.length > 0 ||
                vm.humanResources.employees.length > 0) {
                vm.showDeleteButton = true;
            }
            else {
                vm.showDeleteButton = false;
            }
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

    $scope.$watch(function () {
        return cacheService.getProperty();
    }, function (value) {
        if (value === 1) {
            CommonModel.getAllEmployee().then(function () {
                vm.employees = CommonModel.employees;
            });

            FlowMngModel.resetCurrentFlow(vm);
            vm.showListEmployee = true;
            vm.showDeleteButton = false;
            vm.department = null;
            vm.departmentId = null;
        }
    });

    vm.saveDepartmentFlow = function () {
        if (FlowMngModel.validateDepartmentFlow(vm)) {
            FlowMngModel.createDepartmentFlow(vm);
        }
    };

    vm.deleteDepartmentFlow = function () {
        FlowMngModel.openPopupDeleteDepartmentFlow(vm);
    };

    vm.cancel = function () {
        $window.history.back();
    };
} ]);