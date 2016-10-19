(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('monthlyPeriodModule').controller('UpdateMonthlyPeriodController', updateMonthlyPeriodController);
    updateMonthlyPeriodController.$inject = ['$routeParams', 'MonthlyPeriodModel', 'utilitiesFactory', 'historyFactory'];

    function updateMonthlyPeriodController($routeParams, MonthlyPeriodModel, utilitiesFactory, historyFactory) {
        var vm = this;

        vm.date = new Date();
        vm.period = null;
        vm.permission = null;
        vm.policyEnum = utilitiesFactory.policy;
        vm.periodId = parseInt($routeParams.id);
        vm.viewMode = $routeParams.viewSetting ? true : false;

        vm.dataTab = {
            basicInformation: !vm.viewMode,
            employeeSetting: vm.viewMode,
            processSetting: false
        };

        vm.tabBasicSelect = function () {
            vm.dataTab.basicInformation = true;
            vm.dataTab.employeeSetting = false;
            vm.dataTab.processSetting = false;
        };

        vm.tabEmployeeSettingSelect = function () {
            vm.dataTab.basicInformation = false;
            vm.dataTab.employeeSetting = true;
            vm.dataTab.processSetting = false;
        };

        vm.tabProcessSettingSelect = function () {
            vm.dataTab.basicInformation = false;
            vm.dataTab.employeeSetting = false;
            vm.dataTab.processSetting = true;
        };

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