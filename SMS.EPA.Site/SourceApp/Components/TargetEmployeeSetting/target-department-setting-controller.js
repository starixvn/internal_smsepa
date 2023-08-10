(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetEmployeeSettingModule').controller('TargetDepartmentSettingController', targetDepartmentSettingController);
    targetDepartmentSettingController.$inject = ['$scope', 'TargetEmployeeSettingModel', 'CommonModel', 'TargetTemplateModel',
        'utilitiesFactory', 'lockScreenService'];

    function targetDepartmentSettingController($scope, TargetEmployeeSettingModel, CommonModel, TargetTemplateModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.currentPage = 1;
        vm.departments = CommonModel.departments;
        vm.filter = {};
        vm.isFirstLoad = true;
        vm.pageSize = 20;
        vm.selectedAll = false;
        vm.targetDepartmentSetting = {
            targetTemplateId: null,
            departmentSettings: []
        };
        vm.targetDepartmentSettings = [];
        vm.targetTemplates = [];
        vm.targetTypes = [];
        vm.treeDepartment = CommonModel.treeDepartment;

        vm.assignTargetTemplate = assignTargetTemplate;
        vm.checkAll = checkAll;
        vm.deleteTargetTemplate = deleteTargetTemplate;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.syncSelectedDepartment = syncSelectedDepartment;
        vm.orgChartTemplateSetting = [];

        utilitiesFactory.numberEntryList(vm);
        $scope.$watch(function () { return $scope.targetCtrl.dataTab === 1; },
            function (newVal) {
                if (newVal && vm.isFirstLoad) {
                    initialize();
                }
            }
        );

        function assignTargetTemplate() {
            vm.targetDepartmentSetting.departmentSettings = getSelectedDepartments(vm.targetDepartmentSettings);

            TargetEmployeeSettingModel.assignTargetTemplateToDepartment(vm.targetDepartmentSetting).then(function () {
                vm.query(vm.filter);
                vm.selectedAll = false;
            });
        }

        function checkAll(selectedAll) {
            angular.forEach(vm.filteredItems, function (targetDepartment) {
                targetDepartment.Selected = selectedAll;
            });

            angular.forEach(vm.targetDepartmentSettings, function (targetDepartment) {
                targetDepartment.Selected = selectedAll;
            });
        }

        function deleteTargetTemplate(department, targetTemplate) {
            TargetEmployeeSettingModel.openPopupDeleteTargetDepartmentSetting(targetTemplate, department);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getSelectedDepartments(targetDepartmentSettings) {
            var departments = [];

            angular.forEach(targetDepartmentSettings, function (targetDepartment) {
                if (targetDepartment.Selected) {
                    var department = {
                        DepartmentId: targetDepartment.DepartmentId,
                        IsGroup: targetDepartment.IsGroup
                    };
                    departments.push(department);
                }
            });

            return departments;
        }

        function getTargetDepartmentSetting(departmentIds, targetTypeId, showAll) {
            lockScreenService.start();
            TargetEmployeeSettingModel.getTargetDepartmentSetting(departmentIds, targetTypeId, showAll).then(function () {
                vm.targetDepartmentSettings = TargetEmployeeSettingModel.targetDepartmentSettings;
                utilitiesFactory.searchOnPage(vm, vm.targetDepartmentSettings);
                vm.isFirstLoad = false;
                lockScreenService.stop();
            });
        }

        function initFilter() {
            vm.filter = {
                departmentIds: null,
                isGroup: null,
                targetTypeId: null,
                showAll: false
            };
        }

        function initialize() {
            initFilter();

            CommonModel.GetOrgChartTemplateSetting().then(function () {
                vm.orgChartTemplateSetting = CommonModel.orgChartTemplateSetting;
            });

            CommonModel.getDepartment(null, true).then(function () {
                vm.departments = CommonModel.departments;
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            });

            CommonModel.getTargetType().then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });

            TargetTemplateModel.getTargetTemplate(null, true).then(function () {
                vm.targetTemplates = TargetTemplateModel.targetTemplates;

                if (vm.targetTemplates && vm.targetTemplates.length > 0) {
                    vm.targetDepartmentSetting.targetTemplateId = vm.targetTemplates[0].TargetTemplateId;
                }
            });

            getTargetDepartmentSetting(vm.filter.departmentIds, vm.filter.targetTypeId, vm.filter.showAll);
        }

        function query(filter) {
            vm.searchKey = '';
            vm.currentPage = 1;

            getTargetDepartmentSetting(vm.filter.departmentIds, filter.targetTypeId, filter.showAll);
        }

        function syncSelectedDepartment(department, selected) {
            angular.forEach(vm.targetDepartmentSettings, function (targetDepartment) {
                if (targetDepartment.DepartmentId === department.DepartmentId && targetDepartment.IsGroup === department.IsGroup) {
                    angular.forEach(vm.filteredItems, function (filterItem) {
                        if (targetDepartment.DepartmentId === filterItem.DepartmentId && targetDepartment.IsGroup === filterItem.IsGroup) {
                            filterItem.Selected = selected;
                            targetDepartment.Selected = selected;
                        }
                    });
                }
            });
        }

        vm.onSelect = function (departmentIds) {
            vm.filter.departmentIds = departmentIds;
        };
    }
})(angular);