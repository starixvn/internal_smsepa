(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('UpdateJobLevelStandardKPIController', controller);
    controller.$inject = ['$uibModalInstance', 'JobLevelStandardKPIModel', 'kpi'];

    function controller($uibModalInstance, model, kpi) {
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
            if (model.validateKPI(vm.kpi)) {
                model.updateKPI(vm.kpi, $uibModalInstance);
            }
        }
    }
})(angular);