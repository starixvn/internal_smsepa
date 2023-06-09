(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('UpdateJLSTemplateController', updateController);
    updateController.$inject = ['$routeParams', 'JobLevelStandardTemplateModel', 'historyFactory'];

    function updateController($routeParams, TemplateModel, historyFactory) {
        let vm = this;

        vm.templateId = parseInt($routeParams.id);
        vm.template = null;

        vm.dataTab = $routeParams.viewKPIs ? 2 : 1;

        vm.getTemplateById = function (id) {
            TemplateModel.getTemplateById(id).then(function () {
                vm.template = TemplateModel.template;
            });
        };

        vm.getTemplateById(vm.templateId);

        vm.backToList = function () {
            historyFactory.back();
        };
    }
})(angular);