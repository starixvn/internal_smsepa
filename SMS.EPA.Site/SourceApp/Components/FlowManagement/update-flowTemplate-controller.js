(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('UpdateFlowTemplateController', updateFlowTemplateController);
    updateFlowTemplateController.$inject = ['$routeParams', 'FlowTemplateModel', 'CommonModel', 'utilitiesFactory', 'historyFactory'];

    function updateFlowTemplateController($routeParams, FlowTemplateModel, CommonModel, utilitiesFactory, historyFactory) {
        var vm = this;

        vm.dataTab = 1;
        vm.departments = CommonModel.departments;
        vm.flowTemplateId = parseInt($routeParams.id);
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;
        vm.remindDay = CommonModel.remindDay;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.treeDepartment = CommonModel.treeDepartment;

        vm.backToList = function () {
            historyFactory.back();
        };

        function initialize() {
            CommonModel.getUserPermissionOnFlowManagement().then(function () {
                vm.permission = CommonModel.permission;
            });

            CommonModel.getDepartment().then(function () {
                vm.departments = CommonModel.departments;
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            });

            FlowTemplateModel.getFlowTemplateById(vm.flowTemplateId).then(function () {
                vm.flowTemplate = FlowTemplateModel.flowTemplate;
                vm.flowTemplate.TargetTypeId = FlowTemplateModel.flowTemplate.TargetType.TargetTypeId;
            });
        }

        initialize();
    }
})(angular);