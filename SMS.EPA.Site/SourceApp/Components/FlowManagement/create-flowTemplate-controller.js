(function (angular) {
    'use strict';

    angular.module('flowMngModule').controller('CreateFlowTemplateController', createFlowTemplateController);
    createFlowTemplateController.$inject = ['$modalInstance', 'FlowTemplateModel', 'CommonModel', 'utilitiesFactory'];

    function createFlowTemplateController($modalInstance, FlowTemplateModel, CommonModel, utilitiesFactory) {
        var vm = this;

        vm.flowTemplate = {
            Name: '',
            TargetTypeId: '',
            IsActive: true,
            Description: ''
        };
        vm.policyEnum = utilitiesFactory.policy;
        vm.targetTypes = CommonModel.targetTypes;

        vm.cancel = cancel;
        vm.save = save;

        initialize();

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function initialize() {
            CommonModel.getTargetType(true).then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });
        }

        function save(flowTemplate) {
            if (FlowTemplateModel.validateFlowTemplate(flowTemplate)) {
                FlowTemplateModel.createFlowTemplate(flowTemplate, $modalInstance);
            }
        }
    }
})(angular);