(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('TargetTemplateDetailController', detailController);
    detailController.$inject = ['$routeParams', 'TargetTemplateModel', 'utilitiesFactory', 'historyFactory'];

    function detailController($routeParams, TargetTemplateModel, utilitiesFactory, historyFactory) {
        var vm = this;

        vm.targetTemplateId = parseInt($routeParams.id);
        vm.targetTemplate = null;
        vm.kpiList = null;

        utilitiesFactory.getDateTimeFormat(vm);

        vm.back = function () {
            historyFactory.back();
        };

        function initialize() {
            TargetTemplateModel.getTargetTemplateById(vm.targetTemplateId).then(function () {
                vm.targetTemplate = TargetTemplateModel.targetTemplate;
            });

            TargetTemplateModel.getKPIOfTargetTemplate(vm.targetTemplateId, true).then(function () {
                vm.kpiList = TargetTemplateModel.kpiList;
            });
        }

        initialize();
    }
})(angular);