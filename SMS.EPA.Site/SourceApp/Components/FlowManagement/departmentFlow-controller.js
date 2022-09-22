(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('DepartmentFlowController', departmentFlowController);
    departmentFlowController.$inject = ['$scope', '$routeParams', 'FlowSettingModel', 'DeptFlowModel', 'CommonModel', 'lockScreenService'];

    function departmentFlowController($scope, $routeParams, FlowSettingModel, DeptFlowModel, CommonModel, lockScreenService) {
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
        vm.searchEmployeeEvaluator = '';
        vm.searchEmployeeReviewer = '';
        vm.searchEmployeeManagement = '';
        vm.searchEmployeeHumanResource = '';
        vm.selectedEmployeeEvaluator = null;
        vm.selectedEmployeeReviewer = null;
        vm.selectedEmployeeManagement = null;
        vm.selectedEmployeeHumanResource = null;
        vm.isFirstLoad = true;

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

        vm.adSelectedEmployee = function (sender, selectedEmployee) {
            FlowSettingModel.addEmployee(sender, vm, selectedEmployee);
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

        vm.getEmployee = function (identity) {
            switch (identity) {
                case "evaluatorList":
                    return CommonModel.searchEmployee(vm.searchEmployeeEvaluator);
                    break;
                case "reviewerList":
                    return CommonModel.searchEmployee(vm.searchEmployeeReviewer);
                    break;
                case "managementList":
                    return CommonModel.searchEmployee(vm.searchEmployeeManagement);
                    break;
                case "humanResourceList":
                    return CommonModel.searchEmployee(vm.searchEmployeeHumanResource);
                    break;
            }
        };

        vm.unSelectEmployee = function (identity) {
            switch (identity) {
                case "evaluatorList":
                    if (vm.searchEmployeeEvaluator.trim() === '') {
                        if (vm.selectedEmployeeEvaluator) {
                            vm.selectedEmployeeEvaluator = null;
                        }
                    }
                    break;
                case "reviewerList":
                    if (vm.searchEmployeeReviewer.trim() === '') {
                        if (vm.selectedEmployeeReviewer) {
                            vm.selectedEmployeeReviewer = null;
                        }
                    }
                    break;
                case "managementList":
                    if (vm.searchEmployeeManagement.trim() === '') {
                        if (vm.selectedEmployeeManagement) {
                            vm.selectedEmployeeManagement = null;
                        }
                    }
                    break;
                case "humanResourceList":
                    if (vm.searchEmployeeHumanResource.trim() === '') {
                        if (vm.selectedEmployeeHumanResource) {
                            vm.selectedEmployeeHumanResource = null;
                        }
                    }
                    break;
            }
        };

        function initialize() {
            lockScreenService.start();
            FlowSettingModel.resetCurrentFlow(vm);
            vm.flowTemplate = $scope.updateCtrl.flowTemplate;
            vm.showListEmployee = true;
            vm.showDeleteButton = false;
            vm.department = null;
            vm.departmentId = null;
            vm.isFirstLoad = false;

            DeptFlowModel.getDepartmentHasSetting(vm.FlowTemplateId).then(function () {
                vm.departmentSettings = DeptFlowModel.departmentSettings;
                vm.departmentHasSettingName = getNameDepartment(vm.departmentSettings);
                lockScreenService.stop();
            });
        }

        $scope.$watch(function () { return $scope.updateCtrl.dataTab === 2 && $scope.updateCtrl.flowTemplate; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                initialize();
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
                    vm.showDeleteButton = true;
                    var department = getSelectedDepartment($scope.updateCtrl.departments, vm.department.DepartmentId);

                    var index = vm.departmentSettings.map(function (e) {
                        return e.IsGroup ? -e.DepartmentId : e.DepartmentId;
                    }).indexOf(department.DepartmentId);

                    if (index === -1) {
                        vm.departmentSettings.push(department);
                        vm.departmentHasSettingName = getNameDepartment(vm.departmentSettings);
                    }
                });
            }
        };

        vm.deleteDepartmentFlow = function () {
            DeptFlowModel.openPopupDeleteDepartmentFlow(vm).then(function () {
                vm.showDeleteButton = false;

                var index = vm.departmentSettings.map(function (e) {
                    return e.IsGroup ? -e.DepartmentId : e.DepartmentId;
                }).indexOf(vm.department.DepartmentId);

                vm.departmentSettings.splice(index, 1);
                vm.departmentHasSettingName = getNameDepartment(vm.departmentSettings);
            });
        };
    }
})(angular);