(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('monthlyPeriodModule').controller('UpdateProcessSettingMonthlyPeriodController', updateProcessSettingMonthlyPeriodController);
    updateProcessSettingMonthlyPeriodController.$inject = ['$scope', 'MonthlyPeriodModel', 'CommonModel'];

    function updateProcessSettingMonthlyPeriodController($scope, MonthlyPeriodModel, CommonModel) {
        var vm = this;

        vm.period = {};
        vm.processSettings = [];
        vm.remindDay = CommonModel.remindDay;
        vm.isFirstLoad = true;

        function getProcessSetting(periodId) {
            MonthlyPeriodModel.getProcessSettingByPeriodId(periodId).then(function () {
                vm.processSettings = MonthlyPeriodModel.processSettings;
                vm.isFirstLoad = false;
            });
        }

        $scope.$watch(function () { return $scope.updateCtrl.period && $scope.updateCtrl.dataTab.processSetting; },
        function (newVal) {
            if (newVal && vm.isFirstLoad) {
                vm.period = $scope.updateCtrl.period;
                getProcessSetting(vm.period.PeriodId);
            }
        });

        vm.save = function () {
            var processSetting = {
                PeriodId: vm.period.PeriodId,
                Processes: vm.processSettings
            };

            MonthlyPeriodModel.updateProcessSettingMonthlyPeriod(processSetting);
        };
    }
})(angular);