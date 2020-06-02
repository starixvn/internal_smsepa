(function (angular) {
    'use strict';

    angular.module('feedbackSettingModule').controller('UpdateFeedbackSettingController', updateFeedbackSettingController);
    updateFeedbackSettingController.$inject = ['$uibModalInstance', 'notificationFactory', 'errorFactory', 'feedbackSettingFactory', 'setting'];

    function updateFeedbackSettingController($uibModalInstance, notificationFactory, errorFactory, feedbackSettingFactory, setting) {
        var vm = this;

        vm.isSuccessful = false;
        vm.calendar = {
            fromDateOpened: false,
            toDateOpened: false
        };
        vm.feedbackSetting = {
            employeeId: setting.Employee.EmployeeId,
            employee: setting.Employee.Nickname,
            fromDate: new Date(setting.FromDate),
            toDate: new Date(setting.ToDate)
        };

        vm.openFromDate = function (event) {
            event.preventDefault();
            event.stopPropagation();
            vm.calendar.fromDateOpened = true;
            vm.calendar.toDateOpened = false;
        };

        vm.openToDate = function (event) {
            event.preventDefault();
            event.stopPropagation();
            vm.calendar.fromDateOpened = false;
            vm.calendar.toDateOpened = true;
        };

        vm.save = function () {
            if (new Date(vm.feedbackSetting.fromDate).setHours(0, 0, 0, 0) > new Date(vm.feedbackSetting.toDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error('FromDateGreaterThanToDateWarning');
                return;
            }

            var criteria = {
                periodId: setting.PeriodId,
                employeeIds: vm.feedbackSetting.employeeId,
                fromDate: vm.feedbackSetting.fromDate.toDateString(),
                toDate: vm.feedbackSetting.toDate.toDateString()
            };

            feedbackSettingFactory.updateSetting(criteria).success(function (response) {
                if (response.IsSuccessful) {
                    vm.isSuccessful = true;
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    vm.isSuccessful = false;
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.cancel = function () {
            $uibModalInstance.close(vm.isSuccessful);
        };

        function initialize() {
        }

        initialize();
    }
})(angular);