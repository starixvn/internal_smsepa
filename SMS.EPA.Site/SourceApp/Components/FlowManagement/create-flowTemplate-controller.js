(function (angular) {
    'use strict';
    /*global flowMngModule*/

    angular.module('flowMngModule').controller('CreateFlowTemplateController', createFlowTemplateController);
    createFlowTemplateController.$inject = ['$modalInstance', 'FlowTemplateModel', 'CommonModel', 'enumerationsFactory'];

    function createFlowTemplateController($modalInstance, FlowTemplateModel, CommonModel, enumerationsFactory) {
        var vm = this;

        vm.flowTemplate = {
            Name: '',
            TargetTypeId: '',
            IsActive: true,
            Description: ''
        };
        vm.policyEnum = enumerationsFactory.policy;
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