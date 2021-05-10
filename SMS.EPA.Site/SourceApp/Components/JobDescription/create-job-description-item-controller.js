(function () {
    'use strict';

    angular.module('jobDescriptionModule').controller('CreateDescriptionItemController', createDescriptionItemController);
    createDescriptionItemController.$inject = ['$uibModalInstance', 'JobDescriptionModel', 'groupItemId'];

    function createDescriptionItemController($uibModalInstance, JobDescriptionModel, groupItemId) {
        var vm = this;

        vm.item = {
            Name: '',
            IsActive: true,
            GroupItemId: groupItemId
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