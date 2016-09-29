(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').controller('UpdateFlowTemplateController', updateFlowTemplateController);
    updateFlowTemplateController.$inject = ['$routeParams', 'FlowTemplateModel', 'CommonModel', 'utilitiesFactory'];

    function updateFlowTemplateController($routeParams, FlowTemplateModel, CommonModel, utilitiesFactory) {
        var vm = this;

        vm.dataTab = {
            generalInfo: true,
            department: false,
            personal: false,
            result: false
        };
        vm.departments = CommonModel.departments;
        vm.flowTemplateId = parseInt($routeParams.id);
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;
        vm.remindDay = CommonModel.remindDay;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.treeDepartment = CommonModel.treeDepartment;

        vm.tabGeneralInfoSelected = function () {
            vm.dataTab.generalInfo = true;
            vm.dataTab.department = false;
            vm.dataTab.personal = false;
            vm.dataTab.result = false;
        };

        vm.tabDepartmentSelected = function () {
            vm.dataTab.generalInfo = false;
            vm.dataTab.department = true;
            vm.dataTab.personal = false;
            vm.dataTab.result = false;
        };

        vm.tabPersonalSelected = function () {
            vm.dataTab.generalInfo = false;
            vm.dataTab.department = false;
            vm.dataTab.personal = true;
            vm.dataTab.result = false;
        };

        vm.tabResultSettingSelected = function () {
            vm.dataTab.generalInfo = false;
            vm.dataTab.department = false;
            vm.dataTab.personal = false;
            vm.dataTab.result = true;
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