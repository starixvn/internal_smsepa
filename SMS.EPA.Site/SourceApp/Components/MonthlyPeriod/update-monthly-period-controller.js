(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('monthlyPeriodModule').controller('UpdateMonthlyPeriodController', updateMonthlyPeriodController);
    updateMonthlyPeriodController.$inject = ['$routeParams', 'MonthlyPeriodModel', 'utilitiesFactory', 'historyFactory'];

    function updateMonthlyPeriodController($routeParams, MonthlyPeriodModel, utilitiesFactory, historyFactory) {
        var vm = this;

        vm.date = new Date();
        vm.period = null;
        vm.permission = null;
        vm.policyEnum = utilitiesFactory.policy;
        vm.periodId = parseInt($routeParams.id);
        vm.dataTab = $routeParams.viewSetting ? 3 : 1;

        vm.backToList = function () {
            historyFactory.back();
        };

        function initialize() {
            MonthlyPeriodModel.getPermissionOnMonthlyPeriod().then(function () {
                vm.permission = MonthlyPeriodModel.permission;
            });

            MonthlyPeriodModel.getMonthlyPeriodById(vm.periodId).then(function () {
                vm.period = MonthlyPeriodModel.period;
            });
        }

        initialize();
    }
})(angular);