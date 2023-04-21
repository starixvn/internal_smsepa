(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('UpdateJobLevelStandardKPIController', updateKPIController);
    updateKPIController.$inject = ['$uibModalInstance', 'JobLevelStandardKPIModel', 'kpi'];

    function updateKPIController($uibModalInstance, JobLevelStandardKPIModel, kpi) {
        var vm = this;

        vm.currentChosenKPIGroups = [];
        vm.kpi = {
            KPIId: kpi.KPIId,
            Name: kpi.Name,
            IsActive: kpi.IsActive,
        };

        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            JobLevelStandardKPIModel.updateKPI(vm.kpi, $uibModalInstance);
        }
    }
})(angular);