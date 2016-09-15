(function (angular) {
    'use strict';

    angular.module('flowMngModule').controller('UpdateFlowTemplateBasicInfoController', updateFlowTemplateBasicInfoController);
    updateFlowTemplateBasicInfoController.$inject = ['$scope', '$routeParams', 'FlowTemplateModel', 'CommonModel', 'historyFactory'];

    function updateFlowTemplateBasicInfoController($scope, $routeParams, FlowTemplateModel, CommonModel, historyFactory) {
        var vm = this;

        vm.flowTemplateId = parseInt($routeParams.id);
        vm.targetTypes = CommonModel.targetTypes;

        $scope.$watch(function () { return $scope.updateCtrl.flowTemplate; },
            function (newVal) {
                if (newVal) {
                    vm.flowTemplate = $scope.updateCtrl.flowTemplate;

                    CommonModel.getTargetType(true).then(function () {
                        vm.targetTypes = CommonModel.targetTypes;

                        var index = vm.targetTypes.map(function (e) { return e.TargetTypeId; }).indexOf(vm.flowTemplate.TargetTypeId);
                        if (index === -1) {
                            vm.targetTypes.push(vm.flowTemplate.TargetType);
                        }
                    });
                }
            });

        vm.save = function (flowTemplate) {
            if (FlowTemplateModel.validateFlowTemplate(flowTemplate)) {
                FlowTemplateModel.updateFlowTemplate(flowTemplate);
            }
        };

        vm.cancel = function () {
            historyFactory.back();
        };
    }
})(angular);