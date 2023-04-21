(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('CreateJobLevelStandardKPIController', createKPIController);
    createKPIController.$inject = ['$uibModalInstance', 'JobLevelStandardKPIModel'];

    function createKPIController($uibModalInstance, JobLevelStandardKPIModel) {
        var vm = this;

        vm.kpi = {
            Name: '',
        };
        vm.isDisable = false;
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save(isCreateAndContinued) {
            JobLevelStandardKPIModel.createKPI(vm, isCreateAndContinued, $uibModalInstance);
        }
    }
})(angular);