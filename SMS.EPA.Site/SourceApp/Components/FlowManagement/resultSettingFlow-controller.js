(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('flowMngModule').controller('ResultFlowSettingController', resultFlowSettingController);
    resultFlowSettingController.$inject = ['$scope', '$routeParams', 'DeptFlowModel', 'PersonalFlowModel'];

    function resultFlowSettingController($scope, $routeParams, DeptFlowModel, PersonalFlowModel) {
        var vm = this;

        vm.dataTab = 1;
        vm.flowSettings = [];
        vm.flowTemplateId = parseInt($routeParams.id);

        vm.exportPersonalFlowSetting = exportPersonalFlowSetting;
        vm.tabDepartmentSelected = null;
        vm.tabPersonalSelected = null;

        function exportPersonalFlowSetting() {
            vm.exportCriteria = {
                flowTemplateId: vm.flowTemplateId,
                flowTemplateName: vm.flowTemplate.Name
            };

            $scope.$broadcast('exportPersonalFlowSettingReport');
        }

        function tabDepartmentSelected() {
            vm.dataTab = 1;

            DeptFlowModel.getDepartmentFlow(vm.flowTemplateId).then(function () {
                vm.flowSettings = DeptFlowModel.flowSettings;
            });
        }

        function tabPersonalSelected() {
            vm.dataTab = 2;

            PersonalFlowModel.getPersonalFlow(vm.flowTemplateId).then(function () {
                vm.flowSettings = PersonalFlowModel.flowSettings;
            });
        }

        $scope.$watch(function () { return $scope.updateCtrl.flowTemplate && $scope.updateCtrl.dataTab === 4; },
            function (newVal) {
                if (newVal) {
                    if (vm.dataTab === 1) {
                        tabDepartmentSelected();
                    } else if (vm.dataTab === 2) {
                        tabPersonalSelected();
                    }

                    vm.tabDepartmentSelected = tabDepartmentSelected;
                    vm.tabPersonalSelected = tabPersonalSelected;

                    vm.flowTemplate = $scope.updateCtrl.flowTemplate;
                }
            });
    }
})(angular);