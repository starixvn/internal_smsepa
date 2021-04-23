(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('periodModule').controller('CreatePeriodController', createPeriodController);
    createPeriodController.$inject = ['$uibModalInstance', 'PeriodModel', 'CommonModel', 'utilitiesFactory'];

    function createPeriodController($uibModalInstance, PeriodModel, CommonModel, utilitiesFactory) {
        var vm = this;

        vm.calendar = PeriodModel.calendar;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.targetTypes = [];
        vm.period = {
            Name: null,
            TargetTypeId: null,
            StartDate: null,
            EndDate: null,
            RawStartDate: null,
            RawEndDate: null,
            AttendanceStartDate: null,
            AttendanceEndDate: null,
            IsActive: true,
            Description: null
        };

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

        vm.save = function (period) {
            if (PeriodModel.validatePeriod(period)) {
                period.StartDate = period.RawStartDate ? period.RawStartDate.toDateString() : null;
                period.EndDate = period.RawEndDate ? period.RawEndDate.toDateString() : null;
                period.AttendanceStartDate = period.RawAttendanceStartDate ? period.RawAttendanceStartDate.toDateString() : null;
                period.AttendanceEndDate = period.RawAttendanceEndDate ? period.RawAttendanceEndDate.toDateString() : null;

                PeriodModel.createPeriod(period, $uibModalInstance);
            }
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss("cancel");
        };

        function initialize() {
            CommonModel.getTargetType(true, vm.targetTypeEnum.Monthly).then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });
        }

        initialize();
    }
})(angular);