(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('UpdateJLSTemplateController', updateController);
    updateController.$inject = ['$routeParams', 'JobLevelStandardTemplateModel', 'historyFactory'];

    function updateController($routeParams, JLSTemplateModel, historyFactory) {
        var vm = this;

        vm.templateId = parseInt($routeParams.id);
        vm.template = null;
        vm.originalScoreTypeId = '';

        vm.dataTab = $routeParams.viewKPIs ? 2 : 1;

        vm.getTemplateById = function (id) {
            JLSTemplateModel.getTemplateById(id).then(function () {
                vm.template = JLSTemplateModel.template;
            });
        };

        vm.getTemplateById(vm.templateId);

        vm.backToList = function () {
            historyFactory.back();
        };
    }
})(angular);