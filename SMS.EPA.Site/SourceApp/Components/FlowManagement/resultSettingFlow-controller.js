'use strict';
/*global flowMngModule*/

flowMngModule.controller("ResultSettingFlowController",
['$scope', '$window', '$routeParams', 'FlowMngModel', 'cacheService', 'historyFactory',
function ($scope, $window, $routeParams, FlowMngModel, cacheService, historyFactory) {
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
            FlowMngModel.getDepartmentFlow(vm.flowTemplateId).then(function () {
                vm.flowSettings = FlowMngModel.flowSettings;
            });
        } else if (value === 5) {
            FlowMngModel.getPersonalFlow(vm.flowTemplateId).then(function () {
                vm.flowSettings = FlowMngModel.flowSettings;
            });
        }
    });

    vm.cancel = function () {
        historyFactory.back();
    };
}]);