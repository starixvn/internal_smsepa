(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('monthlyPeriodModule').controller('UpdateBasicInfoMonthlyPeriodController', updateBasicInfoMonthlyPeriodController);
    updateBasicInfoMonthlyPeriodController.$inject = ['$scope', 'MonthlyPeriodModel'];

    function updateBasicInfoMonthlyPeriodController($scope, MonthlyPeriodModel) {
        var vm = this;

        vm.period = {};
        vm.calendar = {
            startDateOpened: false,
            endDateOpened: false
        };

        vm.openPopupStartDate = function (event) {
            MonthlyPeriodModel.openPopupStartDate(event, vm);
        };

        vm.openPopupEndDate = function (event) {
            MonthlyPeriodModel.openPopupEndDate(event, vm);
        };

        $scope.$watch(function () { return $scope.updateCtrl.period && $scope.updateCtrl.dataTab.basicInformation; },
            function (newVal) {
                if (newVal) {
                    vm.period = $scope.updateCtrl.period;
                }
            });

        vm.save = function (period) {
            if (MonthlyPeriodModel.validateMonthlyPeriod(period)) {
                MonthlyPeriodModel.updateBasicInfoMonthlyPeriod(period);
            }
        };
    }
})(angular);