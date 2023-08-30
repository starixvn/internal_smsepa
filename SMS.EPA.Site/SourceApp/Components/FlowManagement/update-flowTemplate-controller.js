(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('UpdateFlowTemplateController', updateFlowTemplateController);
    updateFlowTemplateController.$inject = ['$routeParams', 'FlowTemplateModel', 'CommonModel', 'utilitiesFactory', 'historyFactory'];

    function updateFlowTemplateController($routeParams, FlowTemplateModel, CommonModel, utilitiesFactory, historyFactory) {
        let vm = this;

        vm.dataTab = 1;
        vm.flowTemplateId = parseInt($routeParams.id);
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;
        vm.remindDay = CommonModel.remindDay;
        vm.targetTypeEnum = utilitiesFactory.targetType;

        vm.backToList = function () {
            historyFactory.back();
        };

        function initialize() {
            CommonModel.getUserPermissionOnFlowManagement().then(function () {
                vm.permission = CommonModel.permission;
            });

            FlowTemplateModel.getFlowTemplateById(vm.flowTemplateId).then(function () {
                vm.flowTemplate = FlowTemplateModel.flowTemplate;
                vm.flowTemplate.TargetTypeId = FlowTemplateModel.flowTemplate.TargetType.TargetTypeId;
            });
        }

        initialize();
    }
})(angular);