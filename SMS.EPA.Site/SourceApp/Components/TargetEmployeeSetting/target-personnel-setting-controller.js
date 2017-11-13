(function(angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetEmployeeSettingModule')
        .controller('TargetPersonnelSettingController', targetPersonnelSettingController);
    targetPersonnelSettingController.$inject = [
        '$scope',
        'baseUrl',
        'TargetEmployeeSettingModel',
        'CommonModel',
        'TargetTemplateModel',
        'utilitiesFactory',
        'lockScreenService'
    ];

    function targetPersonnelSettingController(
        $scope,
        baseUrl,
        TargetEmployeeSettingModel,
        CommonModel,
        TargetTemplateModel,
        utilitiesFactory,
        lockScreenService) {

        var vm = this;

        vm.currentPage = 1;
        vm.filter = {};
        vm.groupOptions = [];
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

        $scope.$watch(function() { return $scope.targetCtrl.dataTab === 2; },
            function(newVal) {
                if (newVal && vm.isFirstLoad) {
                    initialize();
                }
            }
        );

        function assignTargetTemplate() {
            vm.targetEmployeeSetting.employeeIds = getSelectedEmployees(vm.targetEmployeeSettings);

            TargetEmployeeSettingModel.assignTargetTemplateToEmployee(vm.targetEmployeeSetting).then(function() {
                vm.query(vm.filter);
                vm.selectedAll = false;
            });
        }

        function checkAll(selectedAll) {
            angular.forEach(vm.filteredItems,
                function(targetEmployee) {
                    targetEmployee.Selected = selectedAll;
                });

            angular.forEach(vm.targetEmployeeSettings,
                function(targetEmployee) {
                    targetEmployee.Selected = selectedAll;
                });
        }

        function deleteTargetTemplate(employee, targetTemplate) {
            TargetEmployeeSettingModel.openPopupDeleteTargetEmployeeSetting(targetTemplate, employee);
        }

        function getEmployee() {
            return CommonModel.getAllEmployee(vm.searchEmployee, vm.filter.groupId);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getSelectedEmployees(targetEmployeeSettings) {
            var employeeIds = '';

            angular.forEach(targetEmployeeSettings,
                function(targetEmployee) {
                    if (targetEmployee.Selected) {
                        employeeIds += employeeIds
                            ? ',' + targetEmployee.Employee.EmployeeId
                            : targetEmployee.Employee.EmployeeId;
                    }
                });

            return employeeIds;
        }

        function getTargetEmployeeSetting(groupId, employeeId, targetTypeId, showAll) {
            lockScreenService.start();
            TargetEmployeeSettingModel.getTargetEmployeeSetting(groupId, employeeId, targetTypeId, showAll).then(
                function() {
                    vm.targetEmployeeSettings = TargetEmployeeSettingModel.targetEmployeeSettings;
                    utilitiesFactory.searchOnPage(vm, vm.targetEmployeeSettings);
                    vm.isFirstLoad = false;
                    lockScreenService.stop();
                });
        }

        function initFilter() {
            vm.filter = {
                groupId: null,
                targetTypeId: null,
                employee: null,
                showAll: false
            };
        }

        function initialize() {
            initFilter();

            CommonModel.getAllGroup().then(function() {
                vm.groupOptions = CommonModel.groups;
            });

            CommonModel.getTargetType().then(function() {
                vm.targetTypes = CommonModel.targetTypes;
            });

            TargetTemplateModel.getTargetTemplate(null, true).then(function() {
                vm.targetTemplates = TargetTemplateModel.targetTemplates;

                if (vm.targetTemplates && vm.targetTemplates.length > 0) {
                    vm.targetEmployeeSetting.targetTemplateId = vm.targetTemplates[0].TargetTemplateId;
                }
            });

            getTargetEmployeeSetting(vm.filter.groupId, null, vm.filter.targetTypeId, vm.filter.showAll);
        }

        function query(filter) {
            var employeeId = null;
            vm.searchKey = '';
            vm.currentPage = 1;

            if (filter.employee) {
                employeeId = filter.employee.EmployeeId;
            }

            getTargetEmployeeSetting(filter.groupId, employeeId, filter.targetTypeId, filter.showAll);
        }

        function syncSelectedEmployee(employeeId, selected) {
            angular.forEach(vm.targetEmployeeSettings,
                function(targetEmployee) {
                    if (targetEmployee.Employee.EmployeeId === employeeId) {
                        angular.forEach(vm.filteredItems,
                            function(filterItem) {
                                if (targetEmployee.Employee.EmployeeId === filterItem.Employee.EmployeeId) {
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

        function exportReport(filter) {
            vm.exportCriteria = angular.copy(filter);

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