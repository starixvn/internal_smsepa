(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('UpdateFlowTemplateBasicInfoController', updateFlowTemplateBasicInfoController);
    updateFlowTemplateBasicInfoController.$inject = ['$scope', '$routeParams', 'FlowTemplateModel', 'CommonModel', 'lockScreenService'];

    function updateFlowTemplateBasicInfoController($scope, $routeParams, FlowTemplateModel, CommonModel, lockScreenService) {
        var vm = this;

        vm.flowTemplateId = parseInt($routeParams.id);
        vm.isFirstLoad = true;
        vm.targetTypes = [];

        $scope.$watch(function () { return $scope.updateCtrl.dataTab.generalInfo && $scope.updateCtrl.flowTemplate; },
            function (newVal) {
                if (newVal && vm.isFirstLoad) {
                    lockScreenService.start();
                    vm.flowTemplate = $scope.updateCtrl.flowTemplate;
                    vm.isFirstLoad = false;

                    CommonModel.getTargetType(true).then(function () {
                        vm.targetTypes = CommonModel.targetTypes;

                        var index = vm.targetTypes.map(function (e) { return e.TargetTypeId; }).indexOf(vm.flowTemplate.TargetTypeId);
                        if (index === -1) {
                            vm.targetTypes.push(vm.flowTemplate.TargetType);
                        }

                        lockScreenService.stop();
                    });
                }
            });

        vm.save = function (flowTemplate) {
            if (FlowTemplateModel.validateFlowTemplate(flowTemplate)) {
                FlowTemplateModel.updateFlowTemplate(flowTemplate);
            }
        };
    }
})(angular);