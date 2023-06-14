(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JLSTemplateDetailController', controller);
    controller.$inject = ['$routeParams', 'JobLevelStandardTemplateModel', 'utilitiesFactory', 'historyFactory'];

    function controller($routeParams, JobLevelStandardTemplateModel, utilitiesFactory, historyFactory) {
        let vm = this;

        vm.targetTemplateId = parseInt($routeParams.id);
        vm.targetTemplate = null;
        vm.kpis = [];

        utilitiesFactory.getDateFormat(vm);

        vm.back = function () {
            historyFactory.back();
        };

        vm.formatWeight = function (weight) {
            return weight % 1 !== 0 ? weight.toFixed(2) : weight
        };

        function initialize() {
            JobLevelStandardTemplateModel.getTemplateById(vm.targetTemplateId).then(function () {
                vm.targetTemplate = JobLevelStandardTemplateModel.template;
            });

            JobLevelStandardTemplateModel.getKPI(vm.targetTemplateId, true).then(function () {
                vm.kpis = JobLevelStandardTemplateModel.kpis;
            });
        }

        initialize();
    }
})(angular);