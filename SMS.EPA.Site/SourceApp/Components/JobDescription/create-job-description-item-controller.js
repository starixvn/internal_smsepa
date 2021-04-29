(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('CreateDescriptionGroupItemController', createDescriptionGroupItemController);
    createDescriptionGroupItemController.$inject = ['$uibModalInstance','JobDescriptionModel'];

    function createDescriptionGroupItemController($uibModalInstance, JobDescriptionModel) {
        var vm = this;

        vm.item = {
            Name: '',
            IsActive: true
        };
        vm.cancel = cancel;
        vm.save = save;

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (JobDescriptionModel.validateDescription(vm.item.Name)) {
                JobDescriptionModel.createDescriptionItem(vm.item, $uibModalInstance);
            }
        }
    }
})(angular);