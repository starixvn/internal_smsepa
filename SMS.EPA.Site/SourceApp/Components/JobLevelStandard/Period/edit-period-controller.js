(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditJobLevelStandardPeriodController', editPeriodController);
    editPeriodController.$inject = ['$routeParams', 'JobLevelStandardPeriodModel', 'utilitiesFactory', 'historyFactory'];

    function editPeriodController($routeParams, PeriodModel, utilitiesFactory, historyFactory) {
        let vm = this;

        vm.permission = PeriodModel.permission;
        vm.policy = utilitiesFactory.policy;
        vm.period = null;
        vm.periodId = parseInt($routeParams.id);
        vm.dataTab = 1;

        vm.goBackToIndexPage = function () {
            historyFactory.back();
        };

        function initialize() {
            PeriodModel.getPermission().then(function () {
                vm.permission = PeriodModel.permission;

                PeriodModel.getPeriodById(vm.periodId).then(function () {
                    vm.period = PeriodModel.period;
                });
            });
        }

        initialize();
    }
})(angular);