(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('CreateJobLevelStandardPeriodController', createPeriodController);
    createPeriodController.$inject = ['$uibModalInstance', 'JobLevelStandardPeriodModel', 'utilitiesFactory'];

    function createPeriodController($uibModalInstance, PeriodModel, utilitiesFactory) {
        let vm = this;

        vm.calendar = PeriodModel.calendar;
        vm.period = {
            Name: '',
            Settings: [
                { ProcessId: utilitiesFactory.jobLevelAssessmentProcess.Evaluator, FromDate: null, ToDate: null },
                { ProcessId: utilitiesFactory.jobLevelAssessmentProcess.Employee, FromDate: null, ToDate: null },
                { ProcessId: utilitiesFactory.jobLevelAssessmentProcess.Management, FromDate: null, ToDate: null }
            ]
        };

        vm.openFromDate = function (event, processId) {
            PeriodModel.openFromDate(event, processId);
        };

        vm.openToDate = function (event, processId) {
            PeriodModel.openToDate(event, processId);
        };

        vm.save = function (period) {
            if (PeriodModel.validatePeriod(period)) {
                let model = angular.copy(period);
                for (let setting of model.Settings) {
                    setting.FromDate = setting.FromDate.toDateString();
                    setting.ToDate = setting.ToDate.toDateString();
                }

                PeriodModel.createPeriod(model, $uibModalInstance);
            }
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);