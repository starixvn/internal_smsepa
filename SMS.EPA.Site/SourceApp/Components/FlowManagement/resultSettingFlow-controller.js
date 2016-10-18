(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').controller('ResultFlowSettingController', resultFlowSettingController);
    resultFlowSettingController.$inject = ['$scope', '$routeParams', 'DeptFlowModel', 'PersonalFlowModel'];

    function resultFlowSettingController($scope, $routeParams, DeptFlowModel, PersonalFlowModel) {
        var vm = this;

        vm.flowSettings = [];
        vm.flowTemplateId = parseInt($routeParams.id);

        vm.tabDepartmentSelected = null;
        vm.tabPersonalSelected = null;

        function tabDepartmentSelected() {
            DeptFlowModel.getDepartmentFlow(vm.flowTemplateId).then(function () {
                vm.flowSettings = DeptFlowModel.flowSettings;
            });
        }

        function tabPersonalSelected() {
            PersonalFlowModel.getPersonalFlow(vm.flowTemplateId).then(function () {
                vm.flowSettings = PersonalFlowModel.flowSettings;
            });
        }

        $scope.$watch(function () { return $scope.updateCtrl.dataTab.result; },
            function (newVal) {
                if (newVal) {
                    tabDepartmentSelected();
                    vm.tabDepartmentSelected = tabDepartmentSelected;
                    vm.tabPersonalSelected = tabPersonalSelected;
                }
            });
    }
})(angular);