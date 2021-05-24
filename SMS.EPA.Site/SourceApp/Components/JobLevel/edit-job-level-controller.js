(function (angular) {
    'use strict';

    angular.module('jobLevelModule').controller('EditJobLevelController', editJobLevelController);
    editJobLevelController.$inject = ['$uibModalInstance', 'JobLevelModel', 'jobLevel'];

    function editJobLevelController($uibModalInstance, JobLevelModel, jobLevel) {
        var vm = this;

        vm.jobLevel = jobLevel;
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobLevelModel.validateJobLevel(vm.jobLevel.Name)) {
                JobLevelModel.editJobLevel(vm.jobLevel, $uibModalInstance);
            }
        }
    }
})(angular);