(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('UpdateTargetTemplateController', updateTargetTemplateController);
    updateTargetTemplateController.$inject = ['$routeParams', 'TargetTemplateModel', 'historyFactory'];

    function updateTargetTemplateController($routeParams, TargetTemplateModel, historyFactory) {
        var vm = this;

        vm.targetTemplateId = parseInt($routeParams.id);
        vm.targetTemplate = null;
        vm.originalScoreTypeId = '';

        vm.dataTab = {
            information: $routeParams.viewInfo ? true : false,
            kpi: $routeParams.viewKPIs ? true : false
        };

        vm.getTargetTemplateById = function (targetTemplateId) {
            TargetTemplateModel.getTargetTemplateById(targetTemplateId).then(function () {
                vm.targetTemplate = TargetTemplateModel.targetTemplate;
                vm.originalScoreTypeId = vm.targetTemplate.ScoreRatingTypeId;
            });
        };

        vm.getTargetTemplateById(vm.targetTemplateId);

        vm.tabInfoSelect = function () {
            vm.dataTab.information = true;
            vm.dataTab.kpi = false;
        };

        vm.tabKPISelect = function () {
            vm.dataTab.information = false;
            vm.dataTab.kpi = true;
        };

        vm.backToList = function () {
            historyFactory.back();
        };
    }
})(angular);