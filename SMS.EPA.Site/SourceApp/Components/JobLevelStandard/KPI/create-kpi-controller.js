(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('CreateJobLevelStandardKPIController', controller);
    controller.$inject = ['$uibModalInstance', 'JobLevelStandardKPIModel'];

    function controller($uibModalInstance, model) {
        let vm = this;

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
            if (model.validateKPI(vm.kpi)) {
                model.createKPI(vm, isCreateAndContinued, $uibModalInstance);
            }
        }
    }
})(angular);