(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetEmployeeSettingModule').controller('TargetSettingTemplateController', targetSettingTemplateController);
    targetSettingTemplateController.$inject = ['TargetEmployeeSettingModel', 'utilitiesFactory'];

    function targetSettingTemplateController(TargetEmployeeSettingModel, utilitiesFactory) {
        var vm = this;

        vm.dataTab = {
            department: true,
            personnel: false
        };
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;

        vm.tabDepartmentSelect = tabDepartmentSelect;
        vm.tabPersonnelSelect = tabPersonnelSelect;

        initialize();

        function initialize() {
            TargetEmployeeSettingModel.getPermissionOnTargetEmployeeSetting().then(function () {
                vm.permission = TargetEmployeeSettingModel.permission;
            });
        }

        function tabDepartmentSelect() {
            vm.dataTab.department = true;
            vm.dataTab.personnel = false;
        }

        function tabPersonnelSelect() {
            vm.dataTab.department = false;
            vm.dataTab.personnel = true;
        }
    }
})(angular);