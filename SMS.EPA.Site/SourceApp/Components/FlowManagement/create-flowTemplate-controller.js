(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('CreateFlowTemplateController', createFlowTemplateController);
    createFlowTemplateController.$inject = ['$uibModalInstance', 'FlowTemplateModel', 'CommonModel', 'utilitiesFactory'];

    function createFlowTemplateController($uibModalInstance, FlowTemplateModel, CommonModel, utilitiesFactory) {
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
            $uibModalInstance.dismiss('cancel');
        }

        function initialize() {
            CommonModel.getTargetType(true, utilitiesFactory.targetType.Feedback360).then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });
        }

        function save(flowTemplate) {
            if (FlowTemplateModel.validateFlowTemplate(flowTemplate)) {
                FlowTemplateModel.createFlowTemplate(flowTemplate, $uibModalInstance);
            }
        }
    }
})(angular);