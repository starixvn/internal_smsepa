(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('periodModule').controller('UpdatePeriodController', updatePeriodController);
    updatePeriodController.$inject = ['$routeParams', 'PeriodModel', 'utilitiesFactory', 'historyFactory'];

    function updatePeriodController($routeParams, PeriodModel, utilitiesFactory, historyFactory) {
        var vm = this;

        vm.date = new Date();
        vm.policyEnum = utilitiesFactory.policy;
        vm.permission = null;
        vm.period = null;
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
            PeriodModel.getPeriodById(vm.periodId).then(function () {
                vm.period = PeriodModel.period;
                vm.period.TargetTypeId = vm.period.TargetType.TargetTypeId;
            });

            PeriodModel.getPermissionOnPeriod().then(function () {
                vm.permission = PeriodModel.permission;
            });
        }

        initialize();
    }
})(angular);