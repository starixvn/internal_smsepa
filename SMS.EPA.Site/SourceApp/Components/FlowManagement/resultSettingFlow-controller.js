(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('flowMngModule').controller('ResultFlowSettingController', resultFlowSettingController);
    resultFlowSettingController.$inject = ['$scope', '$routeParams', 'DeptFlowModel', 'PersonalFlowModel', 'cacheService', 'historyFactory'];

    function resultFlowSettingController($scope, $routeParams, DeptFlowModel, PersonalFlowModel, cacheService, historyFactory) {
        var vm = this;

        vm.flowSettings = [];
        vm.flowTemplateId = parseInt($routeParams.id);

        vm.tabDepartmentSelected = function () {
            cacheService.setProperty(4);
        };

        vm.tabPersonalSelected = function () {
            cacheService.setProperty(5);
        };

        $scope.$watch(function () {
            return cacheService.getProperty();
        }, function (value) {
            if (value === 4) {
                DeptFlowModel.getDepartmentFlow(vm.flowTemplateId).then(function () {
                    vm.flowSettings = DeptFlowModel.flowSettings;
                });
            } else if (value === 5) {
                PersonalFlowModel.getPersonalFlow(vm.flowTemplateId).then(function () {
                    vm.flowSettings = PersonalFlowModel.flowSettings;
                });
            }
        });

        vm.cancel = function () {
            historyFactory.back();
        };
    }
})(angular);