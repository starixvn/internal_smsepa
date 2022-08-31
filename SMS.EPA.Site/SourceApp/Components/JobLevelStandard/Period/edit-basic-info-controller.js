(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('EditBasicInfoJobLevelStandardPeriodController', editBasicInfoPeriodController);
    editBasicInfoPeriodController.$inject = ['$scope', 'JobLevelStandardPeriodModel', 'utilitiesFactory'];

    function editBasicInfoPeriodController($scope, PeriodModel, utilitiesFactory) {
        let vm = this;

        vm.calendar = angular.copy(PeriodModel.calendar);
        vm.process = utilitiesFactory.jobLevelAssessmentProcess;
        vm.period = {};

        vm.openFromDate = function (event, processId) {
            PeriodModel.openFromDate(event, vm.calendar, processId);
        };

        vm.openToDate = function (event, processId) {
            PeriodModel.openToDate(event, vm.calendar, processId);
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
                let model = angular.copy(period);
                for (let setting of model.Settings) {
                    setting.FromDate = setting.FromDate.toDateString();
                    setting.ToDate = setting.ToDate.toDateString();
                }

                PeriodModel.editPeriod(model);
            }
        };
    }
})(angular);