(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetEmployeeSettingModule').controller('TargetPersonnelSettingController', targetPersonnelSettingController);
    targetPersonnelSettingController.$inject = ['$scope', 'baseUrl', 'TargetEmployeeSettingModel',
        'CommonModel', 'TargetTemplateModel', 'utilitiesFactory', 'lockScreenService'];

    function targetPersonnelSettingController($scope, baseUrl, TargetEmployeeSettingModel, CommonModel, TargetTemplateModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.currentPage = 1;
        vm.filter = {};
        vm.isFirstLoad = true;
        vm.pageSize = 20;
        vm.selectedAll = false;
        vm.targetEmployeeSetting = {
            targetTemplateId: null,
            employeeIds: ''
        };
        vm.targetEmployeeSettings = [];
        vm.targetTemplates = [];
        vm.targetTypes = [];
        vm.orgChart = [];

        vm.assignTargetTemplate = assignTargetTemplate;
        vm.checkAll = checkAll;
        vm.deleteTargetTemplate = deleteTargetTemplate;
        vm.getEmployee = getEmployee;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.syncSelectedEmployee = syncSelectedEmployee;
        vm.unSelectEmployee = unSelectEmployee;
        vm.exportReport = exportReport;

        utilitiesFactory.numberEntryList(vm);

        $scope.$watch(function () { return $scope.targetCtrl.dataTab === 2; }, function (newVal) {
            if (newVal && vm.isFirstLoad) {
                initialize();
            }
        });

        function assignTargetTemplate() {
            vm.targetEmployeeSetting.employeeIds = getSelectedEmployees(vm.targetEmployeeSettings);

            TargetEmployeeSettingModel.assignTargetTemplateToEmployee(vm.targetEmployeeSetting).then(function () {
                vm.query(vm.filter);
                vm.selectedAll = false;
            });
        }

        function checkAll(selectedAll) {
            angular.forEach(vm.filteredItems, function (targetEmployee) {
                targetEmployee.Selected = selectedAll;
            });

            angular.forEach(vm.targetEmployeeSettings, function (targetEmployee) {
                targetEmployee.Selected = selectedAll;
            });
        }

        function deleteTargetTemplate(employee, targetTemplate) {
            TargetEmployeeSettingModel.openPopupDeleteTargetEmployeeSetting(targetTemplate, employee);
        }

        function getEmployee() {
            return CommonModel.searchEmployee(vm.searchEmployee, vm.filter.teamIds);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getSelectedEmployees(targetEmployeeSettings) {
            let employeeIds = '';
            angular.forEach(targetEmployeeSettings, function (targetEmployee) {
                if (targetEmployee.Selected) {
                    employeeIds += employeeIds ? ',' + targetEmployee.EmployeeId : targetEmployee.EmployeeId;
                }
            });

            return employeeIds;
        }

        function getTargetEmployeeSetting(teamIds, employeeId, targetTypeId, showAll) {
            lockScreenService.start();
            TargetEmployeeSettingModel.getTargetEmployeeSetting(teamIds, employeeId, targetTypeId, showAll)
                .then(function () {
                    vm.targetEmployeeSettings = TargetEmployeeSettingModel.targetEmployeeSettings;
                    utilitiesFactory.searchOnPage(vm, vm.targetEmployeeSettings);
                    vm.isFirstLoad = false;
                    lockScreenService.stop();
                });
        }

        function initFilter() {
            vm.filter = {
                teamIds: null,
                targetTypeId: null,
                employee: null,
                showAll: false
            };
        }

        function initialize() {
            initFilter();

            CommonModel.getOrgChart().then(function () {
                vm.orgChart = CommonModel.orgChart;
            });

            CommonModel.getTargetType().then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });

            TargetTemplateModel.getTargetTemplate(null, true).then(function () {
                vm.targetTemplates = TargetTemplateModel.targetTemplates;

                if (vm.targetTemplates && vm.targetTemplates.length > 0) {
                    vm.targetEmployeeSetting.targetTemplateId = vm.targetTemplates[0].TargetTemplateId;
                }
            });

            getTargetEmployeeSetting(vm.filter.teamIds, null, vm.filter.targetTypeId, vm.filter.showAll);
        }

        function query(filter) {
            let employeeId = null;
            vm.searchKey = '';
            vm.currentPage = 1;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
            }

            getTargetEmployeeSetting(filter.teamIds, employeeId, filter.targetTypeId, filter.showAll);
        }

        function syncSelectedEmployee(employeeId, selected) {
            angular.forEach(vm.targetEmployeeSettings, function (targetEmployee) {
                if (targetEmployee.EmployeeId === employeeId) {
                    angular.forEach(vm.filteredItems, function (filterItem) {
                        if (targetEmployee.EmployeeId === filterItem.EmployeeId) {
                            filterItem.Selected = selected;
                            targetEmployee.Selected = selected;
                        }
                    });
                }
            });
        }

        function unSelectEmployee() {
            if (vm.searchEmployee.trim() === '') {
                vm.filter.employee = null;
            }
        }

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };

        function exportReport() {
            vm.exportCriteria = angular.copy(vm.filter);

            var url = baseUrl + "ExportReport/ExportTargetPersonnelSettingReport";
            var data = $.param(vm.exportCriteria);

            var downloadUrl = url + "?" + data;
            var iframe = $('body iframe');

            if (iframe.length === 0) {
                iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                iframe.appendTo('body');
            }

            iframe.attr("src", downloadUrl);
        }
    }
})(angular);