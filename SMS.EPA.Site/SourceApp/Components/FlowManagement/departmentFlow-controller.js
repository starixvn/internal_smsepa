(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').controller('DepartmentFlowController', departmentFlowController);
    departmentFlowController.$inject = ['$scope', '$routeParams', 'FlowSettingModel', 'DeptFlowModel', 'CommonModel', 'cacheService', 'historyFactory'];

    function departmentFlowController($scope, $routeParams, FlowSettingModel, DeptFlowModel, CommonModel, cacheService, historyFactory) {
        var vm = this;

        vm.FlowTemplateId = parseInt($routeParams.id);

        vm.selfs = DeptFlowModel.selfs;
        vm.evaluators = DeptFlowModel.evaluators;
        vm.reviewers = DeptFlowModel.reviewers;
        vm.managements = DeptFlowModel.managements;
        vm.humanResources = DeptFlowModel.humanResources;

        vm.departmentSettings = DeptFlowModel.departmentSettings;
        vm.departmentHasSettingName = '';

        vm.employees = [];
        vm.showListEmployee = true;
        vm.showDeleteButton = false;
        vm.department = null;
        vm.departmentId = null;

        function getNameDepartment(departmentSettings) {
            var names = '';

            angular.forEach(departmentSettings, function (departmentSetting) {
                names += names ? ', ' + departmentSetting.Name : departmentSetting.Name;
            });

            return names;
        }

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

        vm.getDepartmentFlowById = function () {
            DeptFlowModel.getDepartmentFlowById(vm.department, vm.FlowTemplateId).then(function () {
                vm.selfs = DeptFlowModel.selfs;
                vm.evaluators = DeptFlowModel.evaluators;
                vm.reviewers = DeptFlowModel.reviewers;
                vm.managements = DeptFlowModel.managements;
                vm.humanResources = DeptFlowModel.humanResources;

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
            FlowSettingModel.removeItemFromList(item, list);
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

        function getEmployeeById(sender, employeeId, position) {
            CommonModel.getEmployeeById(parseInt(employeeId)).then(function () {
                FlowSettingModel.addEmployee(sender, vm, CommonModel.employee, position);
            });
        }

        // On Created list after Drag & Drop.
        $scope.$on('list-created', function (ev, val) {
            getEmployeeById(val.sender, val.id, val.to);
        });

        $scope.$watch(function () {
            return cacheService.getProperty();
        }, function (newVal) {
            if (newVal === 1) {
                CommonModel.getEmployeeByDepartmentId().then(function () {
                    vm.employees = CommonModel.employeesInDepartment;
                });

                DeptFlowModel.getDepartmentHasSetting(vm.FlowTemplateId).then(function () {
                    vm.departmentSettings = DeptFlowModel.departmentSettings;
                    vm.departmentHasSettingName = getNameDepartment(vm.departmentSettings);
                });

                FlowSettingModel.resetCurrentFlow(vm);
                vm.flowTemplate = $scope.updateCtrl.flowTemplate;
                vm.showListEmployee = true;
                vm.showDeleteButton = false;
                vm.department = null;
                vm.departmentId = null;
            }
        });

        function getSelectedDepartment(departments, departmentId) {
            var department = '',
                departmentLength = departments.length;

            for (var i = 0; i < departmentLength; i++) {
                var departmentItem = departments[i];

                if (departmentItem.DepartmentId === departmentId) {
                    department = departmentItem;
                    i = departmentLength;
                }
            }

            return department;
        }

        vm.saveDepartmentFlow = function () {
            if (DeptFlowModel.validateDepartmentFlow(vm)) {
                DeptFlowModel.createDepartmentFlow(vm).then(function () {
                    var department = getSelectedDepartment($scope.updateCtrl.departments, vm.department.DepartmentId);
                    vm.departmentSettings.push(department);
                    vm.departmentHasSettingName = getNameDepartment(vm.departmentSettings);
                });
            }
        };

        vm.deleteDepartmentFlow = function () {
            DeptFlowModel.openPopupDeleteDepartmentFlow(vm).then(function () {
                var index = vm.departmentSettings.map(function (e) {
                    return e.IsGroup ? -e.DepartmentId : e.DepartmentId;
                }).indexOf(vm.department.DepartmentId);

                vm.departmentSettings.splice(index, 1);
                vm.departmentHasSettingName = getNameDepartment(vm.departmentSettings);
            });
        };

        vm.cancel = function () {
            historyFactory.back();
        };
    }
})(angular);