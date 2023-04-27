(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('JLSTemplateDetailController', controller);
    controller.$inject = ['$routeParams', 'JobLevelStandardTemplateModel', 'utilitiesFactory', 'historyFactory'];

    function controller($routeParams, JobLevelStandardTemplateModel, utilitiesFactory, historyFactory) {
        var vm = this;

        vm.targetTemplateId = parseInt($routeParams.id);
        vm.targetTemplate = null;
        vm.kpiList = null;

        utilitiesFactory.getDateFormat(vm);

        vm.back = function () {
            historyFactory.back();
        };

        function initialize() {
            JobLevelStandardTemplateModel.getTemplateById(vm.targetTemplateId).then(function () {
                vm.targetTemplate = JobLevelStandardTemplateModel.template;
            });

            JobLevelStandardTemplateModel.getKPIListSelected(vm.targetTemplateId, true).then(function () {
                vm.kpiList = JobLevelStandardTemplateModel.kpiList;
            });
        }

        initialize();
    }
})(angular);