(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditJobLevelStandardPeriodController', editPeriodController);
    editPeriodController.$inject = ['$routeParams', 'JobLevelStandardPeriodModel', 'utilitiesFactory'];

    function editPeriodController($routeParams, PeriodModel, utilitiesFactory) {
        let vm = this;

        vm.permission = PeriodModel.permission;
        vm.policy = utilitiesFactory.policy;
        vm.period = null;
        vm.periodId = parseInt($routeParams.id);
        vm.dataTab = 1;

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