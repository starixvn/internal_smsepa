(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JLSTemplateSettingController', controller);
    controller.$inject = ['JLSTemplateSettingModel', 'utilitiesFactory'];

    function controller(JLSTemplateSettingModel, utilitiesFactory) {
        var vm = this;

        vm.dataTab = 1;
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;

        initialize();

        function initialize() {
            JLSTemplateSettingModel.getUserPermission().then(function () {
                vm.permission = JLSTemplateSettingModel.permission;
            });
        }
    }
})(angular);