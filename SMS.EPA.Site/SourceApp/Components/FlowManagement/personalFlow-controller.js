(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('PersonalFlowController', personalFlowController);
    personalFlowController.$inject = ['$scope', '$routeParams', 'FlowSettingModel', 'PersonalFlowModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function personalFlowController($scope, $routeParams, FlowSettingModel, PersonalFlowModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.FlowTemplateId = parseInt($routeParams.id);

        vm.selfs = PersonalFlowModel.selfs;
        vm.evaluators = PersonalFlowModel.evaluators;
        vm.reviewers = PersonalFlowModel.reviewers;
        vm.managements = PersonalFlowModel.managements;
        vm.humanResources = PersonalFlowModel.humanResources;

        vm.employees = [];
        vm.employeeIds = '';
        vm.showDeleteButton = false;
        vm.isFirstLoad = true;
        vm.defaultFlowSettings = [];
        vm.personalFlowSettings = [];

        vm.searchEmployeeConfigured = '';
        vm.searchEmployee = '';
        vm.searchEvaluator = '';
        vm.searchReviewer = '';
        vm.searchManagement = '';
        vm.searchHumanResource = '';
        vm.selectedEmployee = null;
        vm.selectedEvaluator = null;
        vm.selectedReviewer = null;
        vm.selectedManagement = null;
        vm.selectedHumanResource = null;

        vm.targetTypeEnum = utilitiesFactory.targetType;

        function getEmployeeFlowSetting() {
            const isMonthly = vm.flowTemplate.IsMonthly ? true : null;
            PersonalFlowModel.getEmployeeFlowSetting(vm.FlowTemplateId, null, isMonthly).then(function () {
                vm.defaultFlowSettings = PersonalFlowModel.defaultFlowSettings;
                vm.personalFlowSettings = PersonalFlowModel.personalFlowSettings;
            });
        }

        /// Remove item from list.
        vm.removeItemFromList = function (item, list) {
            FlowSettingModel.removeItemFromList(item, list);
        };

        vm.adSelectedEmployee = function (sender, selectedEmployee) {
            if (sender === 'employee') {
                vm.getPersonalFlowById(selectedEmployee);
            } else {
                FlowSettingModel.addEmployee(sender, vm, selectedEmployee);
            }

            vm.searchHumanResource = '';
            vm.searchEmployee = '';
            vm.searchEvaluator = '';
            vm.searchReviewer = '';
            vm.searchManagement = '';
            vm.selectedHumanResource = null;
            vm.selectedEmployee = null;
            vm.selectedEvaluator = null;
            vm.selectedReviewer = null;
            vm.selectedManagement = null;
        };

        vm.getEmployee = function (identity) {
            switch (identity) {
                case 'employee':
                    return CommonModel.searchEmployeeByRoleId(vm.searchEmployee, utilitiesFactory.systemRole.Employee);
                    break;
                case 'evaluatorList':
                    return CommonModel.searchEmployeeByRoleId(vm.searchEvaluator, utilitiesFactory.systemRole.Evaluator);
                    break;
                case 'reviewerList':
                    return CommonModel.searchEmployeeByRoleId(vm.searchReviewer, utilitiesFactory.systemRole.Reviewer);
                    break;
                case 'managementList':
                    return CommonModel.searchEmployeeByRoleId(vm.searchManagement, utilitiesFactory.systemRole.MGT);
                    break;
                case 'humanResourceList':
                    return CommonModel.searchEmployeeByRoleId(vm.searchHumanResource, utilitiesFactory.systemRole.HR);
                    break;
            }
        };

        vm.getPersonalFlowById = function (employee) {
            vm.employeeId = employee.EmployeeId;
            vm.employeeIds = employee.EmployeeId.toString();

            PersonalFlowModel.getPersonalFlowById(employee.EmployeeId, vm.FlowTemplateId).then(function () {
                vm.selfs = PersonalFlowModel.selfs;
                vm.evaluators = PersonalFlowModel.evaluators;
                vm.reviewers = PersonalFlowModel.reviewers;
                vm.managements = PersonalFlowModel.managements;
                vm.humanResources = PersonalFlowModel.humanResources;

                if (vm.selfs.employees.length > 0) {
                    vm.showDeleteButton = true;
                    vm.employeeSelected = vm.selfs.employees[0];
                } else {
                    CommonModel.getEmployeeById(employee.EmployeeId).then(function () {
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
                case 'selfList':
                    vm.selfs.employees.splice(val.to, 0, vm.selfs.employees.splice(val.from, 1)[0]);
                    break;
                case 'evaluatorList':
                    vm.evaluators.employees.splice(val.to, 0, vm.evaluators.employees.splice(val.from, 1)[0]);
                    break;
                case 'reviewerList':
                    vm.reviewers.employees.splice(val.to, 0, vm.reviewers.employees.splice(val.from, 1)[0]);
                    break;
                case 'managementList':
                    vm.managements.employees.splice(val.to, 0, vm.managements.employees.splice(val.from, 1)[0]);
                    break;
                case 'humanResourceList':
                    vm.humanResources.employees.splice(val.to, 0, vm.humanResources.employees.splice(val.from, 1)[0]);
                    break;
            }
        });

        $scope.$watch(function () {
            return $scope.updateCtrl.flowTemplate && $scope.updateCtrl.dataTab === 3;
        }, function (newVal) {
            if (newVal && vm.isFirstLoad) {

                vm.isFirstLoad = false;
                FlowSettingModel.resetCurrentFlow(vm);
                vm.flowTemplate = $scope.updateCtrl.flowTemplate;
                vm.showDeleteButton = false;
                vm.defaultFlowSettings = [];
                vm.personalFlowSettings = [];

                getEmployeeFlowSetting();
            }
        });

        vm.savePersonalFlow = function () {
            if (PersonalFlowModel.validatePersonalFlow(vm)) {
                PersonalFlowModel.createPersonalFlow(vm).then(function () {
                    getEmployeeFlowSetting();
                });
            }
        };

        vm.deletePersonalFlow = function () {
            PersonalFlowModel.openPopupDeletePersonalFlow(vm);
        };

        vm.filterEmployeeConfigured = function () {
            return function (item) {
                const filterVal = vm.searchEmployeeConfigured.toLowerCase();
                let matcher = new RegExp(filterVal, 'gi');

                if (filterVal === '' || matcher.test(item.NickName.toLowerCase()) || matcher.test(item.FullName.toLowerCase())) {
                    return item;
                };
            }
        };
    }
})(angular);