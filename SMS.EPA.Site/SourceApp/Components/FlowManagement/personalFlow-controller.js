(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').controller('PersonalFlowController', personalFlowController);
    personalFlowController.$inject = ['$scope', '$routeParams', 'FlowSettingModel', 'PersonalFlowModel', 'CommonModel',
        'utilitiesFactory', 'historyFactory', 'lockScreenService'];

    function personalFlowController($scope, $routeParams, FlowSettingModel, PersonalFlowModel, CommonModel,
        utilitiesFactory, historyFactory, lockScreenService) {
        var vm = this;

        vm.FlowTemplateId = parseInt($routeParams.id);

        vm.selfs = PersonalFlowModel.selfs;
        vm.evaluators = PersonalFlowModel.evaluators;
        vm.reviewers = PersonalFlowModel.reviewers;
        vm.managements = PersonalFlowModel.managements;
        vm.humanResources = PersonalFlowModel.humanResources;

        vm.employees = [];
        vm.showListEmployee = true;
        vm.department = null;
        vm.showDeleteButton = false;
        vm.isFirstLoad = true;
        vm.defaultFlowSettings = [];
        vm.personalFlowSettings = [];

        vm.targetTypeEnum = utilitiesFactory.targetType;

        /// Show or hide list Employee.
        vm.toggleDepartmentDraggableList = function () {
            vm.showListEmployee = !vm.showListEmployee;
        };

        /// Remove item from list.
        vm.removeItemFromList = function (item, list) {
            FlowSettingModel.removeItemFromList(item, list);
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
                PersonalFlowModel.getEmployeeFlowSetting(vm.FlowTemplateId, vm.department.DepartmentId, null).then(function () {
                    vm.defaultFlowSettings = PersonalFlowModel.defaultFlowSettings;
                    vm.personalFlowSettings = PersonalFlowModel.personalFlowSettings;
                });
            }
            else if (vm.department.DepartmentId < 0) {
                PersonalFlowModel.getEmployeeFlowSetting(vm.FlowTemplateId, null, Math.abs(vm.department.DepartmentId)).then(function () {
                    vm.defaultFlowSettings = PersonalFlowModel.defaultFlowSettings;
                    vm.personalFlowSettings = PersonalFlowModel.personalFlowSettings;
                });
            }

            FlowSettingModel.resetCurrentFlow(vm);
            vm.defaultSelect = null;
            vm.personalSelect = null;
            vm.showDeleteButton = false;
        };

        vm.moveItem = function (item, from, to) {
            angular.forEach(item, function (i) {
                var index = from.map(function (e) { return e.EmployeeId; }).indexOf(i);

                if (index !== -1) {
                    to.push(from[index]);
                    from.splice(index, 1);
                }
            });
        };

        vm.moveAll = function (from, to) {
            angular.forEach(from, function (item) {
                to.push(item);
            });

            from.length = 0;
        };

        function getEmployeeById(sender, employeeId, position) {
            CommonModel.getEmployeeById(parseInt(employeeId)).then(function () {
                FlowSettingModel.addEmployee(sender, vm, CommonModel.employee, position);
            });
        }

        vm.getPersonalFlowById = function (item) {
            var employeeId = parseInt(item[0]);
            vm.employeeId = employeeId;

            PersonalFlowModel.getPersonalFlowById(employeeId, vm.FlowTemplateId).then(function () {
                vm.selfs = PersonalFlowModel.selfs;
                vm.evaluators = PersonalFlowModel.evaluators;
                vm.reviewers = PersonalFlowModel.reviewers;
                vm.managements = PersonalFlowModel.managements;
                vm.humanResources = PersonalFlowModel.humanResources;

                if (vm.selfs.employees.length > 0) {
                    vm.showDeleteButton = true;
                    vm.employeeSelected = vm.selfs.employees[0];
                } else {
                    CommonModel.getEmployeeById(employeeId).then(function () {
                        vm.selfs.employees.push(CommonModel.employee);
                        vm.employeeSelected = CommonModel.employee;
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

        $scope.$watch(function () { return $scope.updateCtrl.dataTab.personal && $scope.updateCtrl.flowTemplate; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                lockScreenService.start();

                vm.isFirstLoad = false;
                FlowSettingModel.resetCurrentFlow(vm);
                vm.flowTemplate = $scope.updateCtrl.flowTemplate;
                vm.showListEmployee = true;
                vm.showDeleteButton = false;
                vm.departmentId = null;
                vm.defaultFlowSettings = [];
                vm.personalFlowSettings = [];

                vm.department = $scope.updateCtrl.treeDepartment[0];
                vm.onSelectedDepartment();

                CommonModel.getEmployeeByDepartmentId().then(function () {
                    vm.employees = CommonModel.employeesInDepartment;
                    lockScreenService.stop();
                });
            }
        });

        vm.savePersonalFlow = function () {
            if (PersonalFlowModel.validatePersonalFlow(vm)) {
                PersonalFlowModel.createPersonalFlow(vm);
            }
        };

        vm.deletePersonalFlow = function () {
            PersonalFlowModel.openPopupDeletePersonalFlow(vm);
        };

        vm.cancel = function () {
            historyFactory.back();
        };
    }
})(angular);