(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('monthlyPeriodModule').controller('CreateMonthlyPeriodController', createMonthlyPeriodController);
    createMonthlyPeriodController.$inject = ['$uibModalInstance', 'MonthlyPeriodModel'];

    function createMonthlyPeriodController($uibModalInstance, MonthlyPeriodModel) {
        var vm = this,
        date = new Date();

        vm.period = {
            Name: null,
            StartDate: new Date(date.getFullYear(), date.getMonth(), 1),
            EndDate: new Date(date.getFullYear(), date.getMonth() + 1, 0),
            Description: null
        };

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

        vm.save = function (period) {
            if (MonthlyPeriodModel.validateMonthlyPeriod(period)) {
                period.StartDate = period.StartDate.toDateString();
                period.EndDate = period.EndDate.toDateString();

                MonthlyPeriodModel.createMonthlyPeriod(period, $uibModalInstance);
            }
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);