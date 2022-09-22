(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('PersonalFlowController', personalFlowController);
    personalFlowController.$inject = ['$scope', '$routeParams', 'FlowSettingModel', 'PersonalFlowModel', 'CommonModel',
        'utilitiesFactory', 'lockScreenService', 'baseUrl'];

    function personalFlowController($scope, $routeParams, FlowSettingModel, PersonalFlowModel, CommonModel,
        utilitiesFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.FlowTemplateId = parseInt($routeParams.id);

        vm.selfs = PersonalFlowModel.selfs;
        vm.evaluators = PersonalFlowModel.evaluators;
        vm.reviewers = PersonalFlowModel.reviewers;
        vm.managements = PersonalFlowModel.managements;
        vm.humanResources = PersonalFlowModel.humanResources;

        vm.employees = [];
        vm.employeeIds = '';
        vm.showListEmployee = true;
        vm.department = null;
        vm.showDeleteButton = false;
        vm.isFirstLoad = true;
        vm.defaultFlowSettings = [];
        vm.personalFlowSettings = [];

        vm.searchEmployeeEvaluator = '';
        vm.searchEmployeeReviewer = '';
        vm.searchEmployeeManagement = '';
        vm.searchEmployeeHumanResource = '';
        vm.selectedEmployeeEvaluator = null;
        vm.selectedEmployeeReviewer = null;
        vm.selectedEmployeeManagement = null;
        vm.selectedEmployeeHumanResource = null;

        vm.targetTypeEnum = utilitiesFactory.targetType;

        /// Show or hide list Employee.
        vm.toggleDepartmentDraggableList = function () {
            vm.showListEmployee = !vm.showListEmployee;
        };

        /// Remove item from list.
        vm.removeItemFromList = function (item, list) {
            FlowSettingModel.removeItemFromList(item, list);
        };

        vm.onSelectedDepartment = function () {
            FlowSettingModel.resetCurrentFlow(vm);
            vm.defaultSelect = null;
            vm.personalSelect = null;
            vm.showDeleteButton = false;

            var isMonthlyPeriod = vm.flowTemplate.IsMonthly ? true : null;

            if (vm.department.DepartmentId > 0) {
                PersonalFlowModel.getEmployeeFlowSetting(vm.FlowTemplateId, vm.department.DepartmentId, null, isMonthlyPeriod).then(function () {
                    vm.defaultFlowSettings = PersonalFlowModel.defaultFlowSettings;
                    vm.personalFlowSettings = PersonalFlowModel.personalFlowSettings;
                });
            } else if (vm.department.DepartmentId < 0) {
                PersonalFlowModel.getEmployeeFlowSetting(vm.FlowTemplateId, null, Math.abs(vm.department.DepartmentId), isMonthlyPeriod).then(function () {
                    vm.defaultFlowSettings = PersonalFlowModel.defaultFlowSettings;
                    vm.personalFlowSettings = PersonalFlowModel.personalFlowSettings;
                });
            }
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

        vm.getEmployeeInfo = function (sender, selectedEmployee) {
            FlowSettingModel.addEmployee(sender, vm, selectedEmployee);
        };

        vm.getEmployee = function (identity) {
            switch (identity) {
                case "evaluatorList":
                    return CommonModel.getAllEmployee(vm.searchEmployeeEvaluator);
                    break;
                case "reviewerList":
                    return CommonModel.getAllEmployee(vm.searchEmployeeReviewer);
                    break;
                case "managementList":
                    return CommonModel.getAllEmployee(vm.searchEmployeeManagement);
                    break;
                case "humanResourceList":
                    return CommonModel.getAllEmployee(vm.searchEmployeeHumanResource);
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

        vm.getPersonalFlowById = function (item) {
            if (item && item.length > 0) {
                if (item.length === 1) {
                    var employeeId = parseInt(item[0]);
                    vm.employeeId = employeeId;
                    vm.employeeIds = employeeId.toString();

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
                } else {
                    FlowSettingModel.resetCurrentFlow(vm);
                    vm.selfs.employees = [{
                        EmployeeId: -1,
                        NickName: 'Self',
                        Avatar: baseUrl + 'avatars/avatar.jpg'
                    }];
                    vm.employeeIds = item.join(',');
                }
            }
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

        $scope.$watch(function () { return $scope.updateCtrl.flowTemplate && $scope.updateCtrl.dataTab === 3; },
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
    }
})(angular);