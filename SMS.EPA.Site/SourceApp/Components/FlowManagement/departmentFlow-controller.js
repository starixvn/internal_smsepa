(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('DepartmentFlowController', departmentFlowController);
    departmentFlowController.$inject = ['$scope', '$routeParams', 'FlowSettingModel', 'DeptFlowModel', 'CommonModel', 'lockScreenService'];

    function departmentFlowController($scope, $routeParams, FlowSettingModel, DeptFlowModel, CommonModel, lockScreenService) {
        let vm = this;

        vm.FlowTemplateId = parseInt($routeParams.id);

        vm.selfs = DeptFlowModel.selfs;
        vm.evaluators = DeptFlowModel.evaluators;
        vm.reviewers = DeptFlowModel.reviewers;
        vm.managements = DeptFlowModel.managements;
        vm.humanResources = DeptFlowModel.humanResources;

        vm.departmentSettings = DeptFlowModel.departmentSettings;

        vm.employees = [];
        vm.showDeleteButton = false;
        vm.selectedTeam = null;
        vm.searchEvaluator = '';
        vm.searchReviewer = '';
        vm.searchManagement = '';
        vm.searchHumanResource = '';
        vm.selectedEvaluator = null;
        vm.selectedReviewer = null;
        vm.selectedManagement = null;
        vm.selectedHumanResource = null;
        vm.isFirstLoad = true;
        vm.departments = [];

        vm.checkTeamHasSetting = function (teamId) {
            return vm.departmentSettings.find(team => team.DepartmentId === -teamId) !== undefined;
        };

        vm.getDepartmentFlowById = function (team) {
            vm.selectedTeam = team;
            DeptFlowModel.getDepartmentFlowById(team, vm.FlowTemplateId).then(function () {
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
                } else {
                    vm.showDeleteButton = false;
                }
            });
        };

        vm.adSelectedEmployee = function (sender, selectedEmployee) {
            FlowSettingModel.addEmployee(sender, vm, selectedEmployee);

            vm.searchHumanResource = '';
            vm.searchEvaluator = '';
            vm.searchReviewer = '';
            vm.searchManagement = '';
            vm.selectedHumanResource = null;
            vm.selectedEvaluator = null;
            vm.selectedReviewer = null;
            vm.selectedManagement = null;
        };

        /// Remove item from list.
        vm.removeItemFromList = function (item, list) {
            FlowSettingModel.removeItemFromList(item, list);
        };

        // Sort item in list.
        $scope.$on('list-sorted', function (ev, val) {
            switch (val.sender) {
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
                    return CommonModel.searchEmployee(vm.searchEvaluator);

                case "reviewerList":
                    return CommonModel.searchEmployee(vm.searchReviewer);

                case "managementList":
                    return CommonModel.searchEmployee(vm.searchManagement);

                case "humanResourceList":
                    return CommonModel.searchEmployee(vm.searchHumanResource);
            }
        };

        vm.changeCollapseAttribute = function (item) {
            item.Collapse = !item.Collapse;
        };

        function initialize() {
            lockScreenService.start();
            FlowSettingModel.resetCurrentFlow(vm);
            vm.flowTemplate = $scope.updateCtrl.flowTemplate;
            vm.showDeleteButton = false;
            vm.selectedTeam = null;
            vm.isFirstLoad = false;

            getDepartmentHasSetting();
            getOnlyDepartment($scope.updateCtrl.treeDepartment)
            addCollapseAttribute(vm.departments)
        }

        function getDepartmentHasSetting() {
            DeptFlowModel.getDepartmentHasSetting(vm.FlowTemplateId).then(function () {
                vm.departmentSettings = DeptFlowModel.departmentSettings;
                lockScreenService.stop();
            });
        }

        function addCollapseAttribute(departments) {
            departments.forEach(dept => {
                dept.Collapse = false;
                dept.Childrens.forEach(sec => {
                    sec.Collapse = false;
                })
            })
        }

        function getOnlyDepartment(departments) {
            departments.Childrens.forEach(unit => {
                unit.Childrens.forEach(dept => {
                    vm.departments.push(dept)
                })
            });
        }

        $scope.$watch(function () {
            return $scope.updateCtrl.dataTab === 2 && $scope.updateCtrl.flowTemplate;
        }, function (newVal) {
            if (newVal && vm.isFirstLoad) {
                initialize();
            }
        });

        vm.saveDepartmentFlow = function () {
            if (DeptFlowModel.validateDepartmentFlow(vm)) {
                DeptFlowModel.createDepartmentFlow(vm).then(function () {
                    vm.showDeleteButton = true;
                    getDepartmentHasSetting();
                });
            }
        };

        vm.deleteDepartmentFlow = function () {
            DeptFlowModel.openPopupDeleteDepartmentFlow(vm).then(function () {
                vm.showDeleteButton = false;
                getDepartmentHasSetting();
            });
        };
    }
})(angular);