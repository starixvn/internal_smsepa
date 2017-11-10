(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetEmployeeSettingModule').controller('TargetSettingTemplateController', targetSettingTemplateController);
    targetSettingTemplateController.$inject = ['TargetEmployeeSettingModel', 'utilitiesFactory'];

    function targetSettingTemplateController(TargetEmployeeSettingModel, utilitiesFactory) {
        var vm = this;

        vm.dataTab = 1;
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;

        initialize();

        function initialize() {
            TargetEmployeeSettingModel.getPermissionOnTargetEmployeeSetting().then(function () {
                vm.permission = TargetEmployeeSettingModel.permission;
            });
        }
    }
})(angular);