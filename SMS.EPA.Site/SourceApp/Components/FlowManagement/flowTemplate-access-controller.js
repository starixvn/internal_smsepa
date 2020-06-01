(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('FlowTemplateController', flowTemplateController);
    flowTemplateController.$inject = ['$location', 'FlowTemplateModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService', 'baseUrl'];

    function flowTemplateController($location, FlowTemplateModel, CommonModel, utilitiesFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.currentPage = 1;
        vm.flowTemplates = [];
        vm.filter = {
            targetTypeId: '',
            isActive: 'true'
        };
        vm.pageSize = 20;
        vm.permission = 0;
        vm.policyEnum = utilitiesFactory.policy;
        vm.targetTypes = [];

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.maxLengthDescription(vm);

        vm.createFlowTemplate = createFlowTemplate;
        vm.deleteFlowTemplate = deleteFlowTemplate;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.readLess = readLess;
        vm.readMore = readMore;
        vm.updateFlowTemplate = updateFlowTemplate;
        vm.updateStatusFlowTemplate = updateStatusFlowTemplate;

        initialize();

        function createFlowTemplate() {
            FlowTemplateModel.openPopupCreateFlowTemplate(vm).then(function () {
                vm.modalInstance = null;
                vm.query(vm.filter);
            });
        }

        function deleteFlowTemplate(flowTemplate) {
            FlowTemplateModel.openPopupDeleteFlowTemplate(vm, flowTemplate);
        }

        function getFlowTemplate(targetTypeId, isActive) {
            lockScreenService.start();
            FlowTemplateModel.getFlowTemplate(targetTypeId, isActive).then(function () {
                vm.flowTemplates = FlowTemplateModel.flowTemplates;
                utilitiesFactory.searchOnPage(vm, vm.flowTemplates);
                lockScreenService.stop();
            });
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function initialize() {
            CommonModel.getUserPermissionOnFlowManagement().then(function () {
                vm.permission = CommonModel.permission;
            });

            CommonModel.getTargetType(true, utilitiesFactory.targetType.Feedback360).then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });

            getFlowTemplate(vm.filter.targetTypeId, vm.filter.isActive);
        }

        function query(filter) {
            vm.searchKey = '';
            vm.currentPage = 1;
            getFlowTemplate(filter.targetTypeId, filter.isActive);
        }

        function readLess(item) {
            utilitiesFactory.readLess(item);
        }

        function readMore(item) {
            utilitiesFactory.readMore(item);
        }

        function updateFlowTemplate(flowTemplate) {
            $location.path(baseUrl + 'Flow/Update/' + flowTemplate.FlowTemplateId);
        }

        function updateStatusFlowTemplate(flowTemplate) {
            FlowTemplateModel.openPopupUpdateStatusFlowTemplate(flowTemplate);
        }
    }
})(angular);