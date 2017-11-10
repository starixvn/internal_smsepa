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

        vm.dataTab = $routeParams.viewKPIs ? 2 : 1;

        vm.getTargetTemplateById = function (targetTemplateId) {
            TargetTemplateModel.getTargetTemplateById(targetTemplateId).then(function () {
                vm.targetTemplate = TargetTemplateModel.targetTemplate;
                vm.originalScoreTypeId = vm.targetTemplate.ScoreRatingTypeId;
            });
        };

        vm.getTargetTemplateById(vm.targetTemplateId);

        vm.backToList = function () {
            historyFactory.back();
        };
    }
})(angular);