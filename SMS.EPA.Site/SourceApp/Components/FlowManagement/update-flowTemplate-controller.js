(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').controller('UpdateFlowTemplateController', updateFlowTemplateController);
    updateFlowTemplateController.$inject = ['$routeParams', 'FlowTemplateModel', 'CommonModel', 'utilitiesFactory', 'cacheService'];

    function updateFlowTemplateController($routeParams, FlowTemplateModel, CommonModel, utilitiesFactory, cacheService) {
        var vm = this;

        vm.flowTemplateId = parseInt($routeParams.id);
        vm.departments = CommonModel.departments;
        vm.treeDepartment = CommonModel.treeDepartment;
        vm.remindDay = CommonModel.remindDay;
        vm.permission = FlowTemplateModel.permission;

        vm.policyEnum = utilitiesFactory.policy;
        vm.targetTypeEnum = utilitiesFactory.targetType;

        vm.tabDepartmentSelected = function () {
            cacheService.setProperty(1);
        };

        vm.tabPersonalSelected = function () {
            cacheService.setProperty(2);
        };

        vm.tabResultSettingSelected = function () {
            cacheService.setProperty(3);
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