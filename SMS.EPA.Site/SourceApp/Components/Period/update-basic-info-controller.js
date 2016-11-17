(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('periodModule').controller('UpdateBasicInformationPeriodController', updateBasicInformationPeriodController);
    updateBasicInformationPeriodController.$inject = ['$scope', 'PeriodModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function updateBasicInformationPeriodController($scope, PeriodModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.calendar = PeriodModel.calendar;
        vm.isFirstLoad = true;
        vm.period = {};
        vm.targetTypes = [];
        vm.targetTypeEnum = utilitiesFactory.targetType;

        vm.openPopupStartDate = function (event) {
            PeriodModel.openPopupStartDate(event);
        };

        vm.openPopupEndDate = function (event) {
            PeriodModel.openPopupEndDate(event);
        };

        vm.openPopupAttendanceStartDate = function (event) {
            PeriodModel.openPopupAttendanceStartDate(event);
        };

        vm.openPopupAttendanceEndDate = function (event) {
            PeriodModel.openPopupAttendanceEndDate(event);
        };

        function getTargetType() {
            lockScreenService.start();

            CommonModel.getTargetType(true, vm.targetTypeEnum.Monthly).then(function () {
                vm.targetTypes = CommonModel.targetTypes;
                vm.isFirstLoad = false;
                lockScreenService.stop();
            });
        }

        $scope.$watch(function () { return $scope.updateCtrl.period && $scope.updateCtrl.dataTab.basicInformation; },
            function (newVal) {
                if (newVal) {
                    vm.period = $scope.updateCtrl.period;
                    vm.period.RawStartDate = vm.period.StartDate ? new Date(vm.period.StartDate) : null;
                    vm.period.RawEndDate = vm.period.EndDate ? new Date(vm.period.EndDate) : null;
                    vm.period.RawAttendanceStartDate = vm.period.AttendanceStartDate ? new Date(vm.period.AttendanceStartDate) : null;
                    vm.period.RawAttendanceEndDate = vm.period.AttendanceEndDate ? new Date(vm.period.AttendanceEndDate) : null;

                    if (vm.isFirstLoad && vm.period.PeriodId) {
                        getTargetType();
                    }
                }
            });

        vm.save = function (period) {
            if (PeriodModel.validatePeriod(period)) {
                period.StartDate = period.RawStartDate ? period.RawStartDate.toDateString() : null;
                period.EndDate = period.RawEndDate ? period.RawEndDate.toDateString() : null;
                period.AttendanceStartDate = period.RawAttendanceStartDate ? period.RawAttendanceStartDate.toDateString() : null;
                period.AttendanceEndDate = period.RawAttendanceEndDate ? period.RawAttendanceEndDate.toDateString() : null;

                PeriodModel.updateBasicInformationPeriod(period);
            }
        };
    }
})(angular);