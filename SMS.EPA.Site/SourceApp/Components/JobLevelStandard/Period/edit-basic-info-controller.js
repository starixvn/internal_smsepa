(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditBasicInfoJobLevelStandardPeriodController', editBasicInfoPeriodController);
    editBasicInfoPeriodController.$inject = ['$scope', 'JobLevelStandardPeriodModel', 'CommonModel', 'lockScreenService'];

    function editBasicInfoPeriodController($scope, PeriodModel, CommonModel, lockScreenService) {
        var vm = this;

        vm.calendar = PeriodModel.calendar;
        vm.period = {};

        vm.openFromDate = function (event, processId) {
            PeriodModel.openFromDate(event, processId);
        };

        vm.openToDate = function (event, processId) {
            PeriodModel.openToDate(event, processId);
        };

        $scope.$watch(function () { return $scope.editCtrl.period && $scope.editCtrl.dataTab === 1; },
            function (newVal) {
                if (newVal) {
                    vm.period = angular.copy($scope.editCtrl.period);
                    for (var setting of vm.period.Settings) {
                        setting.FromDate = new Date(setting.FromDate);
                        setting.ToDate = new Date(setting.ToDate);
                    }
                }
            });

        vm.save = function (period) {
            if (PeriodModel.validatePeriod(period)) {
                var model = angular.copy(period);
                for (var setting of model.Settings) {
                    setting.FromDate = setting.FromDate.toDateString();
                    setting.ToDate = setting.ToDate.toDateString();
                }

                PeriodModel.editPeriod(model);
            }
        };
    }
})(angular);