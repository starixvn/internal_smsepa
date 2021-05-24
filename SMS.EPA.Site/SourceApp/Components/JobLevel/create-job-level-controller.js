(function (angular) {
    'use strict';

    angular.module('jobLevelModule').controller('CreateJobLevelController', createJobLevelController);
    createJobLevelController.$inject = ['$uibModalInstance', 'JobLevelModel'];

    function createJobLevelController($uibModalInstance, JobLevelModel) {
        var vm = this;

        vm.jobLevel = {
            Name: ''
        };
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobLevelModel.validateJobLevel(vm.jobLevel.Name)) {
                JobLevelModel.createJobLevel(vm.jobLevel, $uibModalInstance);
            }
        }
    }
})(angular);